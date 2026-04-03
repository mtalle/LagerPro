'use client';
import { useEffect, useState, Fragment } from 'react';
import { LagerBeholdning, get, patch, getMe } from '../../lib/api';

interface Lageroversigt {
  artikkelId: number;
  artikkelNr: string;
  artikkelNavn: string;
  enhet: string;
  totalMengde: number;
  antallLots: number;
  minBeholdning?: number;
  detaljer: LagerBeholdning[];
}

interface VaretellingRad {
  artikkelId: number;
  artikkelNr: string;
  artikkelNavn: string;
  enhet: string;
  systemMengde: number;
  faktiskMengde: string;
  differanse: number;
}

export default function LagerPage() {
  const [beholdninger, setBeholdninger] = useState<LagerBeholdning[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [visKunLav, setVisKunLav] = useState(false);
  const [adjustTarget, setAdjustTarget] = useState<LagerBeholdning | null>(null);
  const [adjustMengde, setAdjustMengde] = useState('');
  const [adjustKommentar, setAdjustKommentar] = useState('');
  const [adjustError, setAdjustError] = useState('');
  const [adjustSuccess, setAdjustSuccess] = useState('');
  const [adjustLoading, setAdjustLoading] = useState(false);

  // Full varetelling modal
  const [varetellingOpen, setVaretellingOpen] = useState(false);
  const [varetellingData, setVaretellingData] = useState<VaretellingRad[]>([]);
  const [varetellingLoading, setVaretellingLoading] = useState(false);
  const [varetellingConfirm, setVaretellingConfirm] = useState(false);
  const [varetellingSubmitting, setVaretellingSubmitting] = useState(false);
  const [varetellingOk, setVaretellingOk] = useState(false);
  const [varetellingError, setVaretellingError] = useState('');

  // Expanded article rows
  const [expandedArtikkel, setExpandedArtikkel] = useState<number | null>(null);

  useEffect(() => {
    async function init() { try { await getMe(); } catch { return; } loadData(); }
    init();
  }, []);

  async function loadData() {
    try {
      const data = await get<LagerBeholdning[]>('/inventory');
      setBeholdninger(data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  function openAdjust(b: LagerBeholdning) {
    setAdjustTarget(b);
    setAdjustMengde(b.mengde.toString());
    setAdjustKommentar('');
    setAdjustError('');
    setAdjustSuccess('');
  }

  async function handleAdjust(e: React.FormEvent) {
    e.preventDefault();
    if (!adjustTarget) return;
    const nyMengde = parseFloat(adjustMengde);
    if (isNaN(nyMengde) || nyMengde < 0) { setAdjustError('Ugyldig mengde.'); return; }
    setAdjustLoading(true);
    setAdjustError('');
    try {
      await patch('/inventory/juster', {
        artikkelId: adjustTarget.artikkelId,
        lotNr: adjustTarget.lotNr,
        nyMengde,
        kommentar: adjustKommentar || null,
      });
      setAdjustSuccess(`Beholdning justert fra ${adjustTarget.mengde} til ${nyMengde}.`);
      setTimeout(() => { setAdjustTarget(null); setAdjustSuccess(''); }, 2000);
      loadData();
    } catch (err) {
      setAdjustError('Feil: ' + (err as Error).message);
    } finally {
      setAdjustLoading(false);
    }
  }

  async function openVaretelling() {
    setVaretellingLoading(true);
    setVaretellingOpen(true);
    setVaretellingConfirm(false);
    setVaretellingOk(false);
    setVaretellingError('');
    try {
      const data = await get<Lageroversigt[]>('/inventory/oversikt');
      const rader: VaretellingRad[] = data.map(a => ({
        artikkelId: a.artikkelId,
        artikkelNr: a.artikkelNr,
        artikkelNavn: a.artikkelNavn,
        enhet: a.enhet,
        systemMengde: a.totalMengde,
        faktiskMengde: '',
        differanse: 0,
      }));
      setVaretellingData(rader);
    } catch {
      setVaretellingData([]);
    } finally {
      setVaretellingLoading(false);
    }
  }

  function updateFaktiskMengde(artikkelId: number, verdi: string) {
    setVaretellingData(rader =>
      rader.map(r => {
        if (r.artikkelId !== artikkelId) return r;
        const fm = verdi === '' ? 0 : parseFloat(verdi);
        return { ...r, faktiskMengde: verdi, differanse: fm - r.systemMengde };
      })
    );
  }

  async function submitVaretelling() {
    setVaretellingSubmitting(true);
    try {
      for (const rad of varetellingData) {
        if (rad.faktiskMengde === '' || rad.differanse === 0) continue;
        const lotter = beholdninger.filter(b => b.artikkelId === rad.artikkelId);
        for (const lot of lotter) {
          const nyMengde = Math.max(0, lot.mengde + rad.differanse / lotter.length);
          await patch('/inventory/juster', {
            artikkelId: rad.artikkelId,
            lotNr: lot.lotNr,
            nyMengde,
            kommentar: `Varetelling ${new Date().toLocaleDateString('no-NO')}`,
          });
        }
      }
      setVaretellingOk(true);
      setVaretellingError('');
      setTimeout(() => { setVaretellingOpen(false); loadData(); }, 1500);
    } catch (err) {
      setVaretellingError('Feil ved lagring: ' + (err as Error).message);
    } finally {
      setVaretellingSubmitting(false);
    }
  }

  // Group beholdninger by article for display
  const groupedByArticle = beholdninger.reduce((acc, b) => {
    if (!acc[b.artikkelId]) {
      acc[b.artikkelId] = {
        artikkelId: b.artikkelId,
        artikkelNr: b.artikkelNr,
        artikkelNavn: b.artikkelNavn,
        enhet: b.enhet,
        totalMengde: 0,
        minBeholdning: b.minBeholdning,
        lots: [],
      };
    }
    acc[b.artikkelId].lots.push(b);
    acc[b.artikkelId].totalMengde += b.mengde;
    return acc;
  }, {} as Record<number, { artikkelId: number; artikkelNr: string; artikkelNavn: string; enhet: string; totalMengde: number; minBeholdning?: number; lots: LagerBeholdning[] }>);

  const articleList = Object.values(groupedByArticle).sort((a, b) => a.artikkelNr.localeCompare(b.artikkelNr));

  const filtered = articleList.filter(a => {
    const matchesSearch =
      a.artikkelNavn.toLowerCase().includes(search.toLowerCase()) ||
      a.artikkelNr.toLowerCase().includes(search.toLowerCase()) ||
      a.lots.some(l => l.lotNr.toLowerCase().includes(search.toLowerCase())) ||
      (a.lots[0]?.lokasjon ?? '').toLowerCase().includes(search.toLowerCase());

    const erLav = a.minBeholdning != null && a.totalMengde < a.minBeholdning;
    return matchesSearch && (!visKunLav || erLav);
  });

  if (loading) return <div className="loading">Laster lager...</div>;

  return (
    <>
      <div className="page-header">
        <h1>🏭 Lagerbeholdning</h1>
        <button className="btn btn-secondary" onClick={openVaretelling}>📦 Full varetelling</button>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Søk artikkel, lotnr eller lokasjon..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <label className="filter-label">
          <input type="checkbox" checked={visKunLav} onChange={e => setVisKunLav(e.target.checked)} />
          Vis kun lav beholdning
        </label>
        <span className="filter-count">{filtered.length} av {beholdninger.length}</span>
      </div>

      <table className="table-scroll">
        <thead>
          <tr>
            <th></th>
            <th>Artikkelnr</th>
            <th>Navn</th>
            <th>Lot</th>
            <th>Mengde</th>
            <th>Enhet</th>
            <th>Min</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={9} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen beholdning funnet</td></tr>
          ) : filtered.map(a => {
            const erLav = a.minBeholdning != null && a.totalMengde < a.minBeholdning;
            const isExpanded = expandedArtikkel === a.artikkelId;
            return (
              <Fragment key={a.artikkelId}>
                <tr
                  style={erLav ? { background: '#fef2f2', cursor: 'pointer' } : { cursor: 'pointer' }}
                  onClick={() => setExpandedArtikkel(isExpanded ? null : a.artikkelId)}
                >
                  <td style={{ width: 32, textAlign: 'center' }}>
                    {erLav && <span title={`Lav beholdning — minst ${a.minBeholdning} ${a.enhet}`}>⚠️</span>}
                  </td>
                  <td><code>{a.artikkelNr}</code></td>
                  <td>{a.artikkelNavn}</td>
                  <td>
                    <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
                      {isExpanded ? '▼' : '▶'}
                      <code>{a.lots.length} lot{a.lots.length !== 1 ? 'ter' : ''}</code>
                    </span>
                  </td>
                  <td><strong style={erLav ? { color: '#dc2626' } : undefined}>{a.totalMengde}</strong></td>
                  <td>{a.enhet}</td>
                  <td style={{ color: erLav ? '#dc2626' : '#16a34a', fontSize: '0.85rem' }}>
                    {a.minBeholdning != null ? a.minBeholdning : '—'}
                  </td>
                  <td>
                    <button
                      className="btn btn-sm btn-secondary"
                      onClick={e => { e.stopPropagation(); openAdjust(a.lots[0]); }}
                    >Juster</button>
                  </td>
                </tr>
                {isExpanded && a.lots.map(lot => {
                  const lotErLav = lot.minBeholdning != null && lot.mengde < lot.minBeholdning;
                  return (
                    <tr key={lot.id} style={{ background: '#f9fafb', fontSize: '0.9rem' }}>
                      <td />
                      <td />
                      <td style={{ paddingLeft: '1.5rem', color: '#6b7280' }}>
                        <code>{lot.lotNr}</code>
                        {lot.lokasjon && (
                          <span style={{ marginLeft: 8, fontSize: '0.8rem', background: '#e5e7eb', padding: '1px 5px', borderRadius: 3 }}>
                            {lot.lokasjon}
                          </span>
                        )}
                      </td>
                      <td />
                      <td style={lotErLav ? { color: '#dc2626' } : undefined}>{lot.mengde}</td>
                      <td>{lot.enhet}</td>
                      <td>
                        {lot.bestForDato
                          ? new Date(lot.bestForDato).toLocaleDateString('no-NO')
                          : <span style={{ color: '#d1d5db' }}>—</span>
                        }
                      </td>
                      <td>
                        <button
                          className="btn btn-sm btn-secondary"
                          style={{ fontSize: '0.75rem', padding: '2px 8px' }}
                          onClick={e => { e.stopPropagation(); openAdjust(lot); }}
                        >Juster</button>
                      </td>
                    </tr>
                  );
                })}
              </Fragment>
            );
          })}
        </tbody>
      </table>

      {adjustTarget && (
        <div className="modal-overlay" onClick={() => setAdjustTarget(null)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 420 }}>
            <h2>Juster beholdning</h2>
            <p style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '1rem' }}>
              {adjustTarget.artikkelNavn} ({adjustTarget.artikkelNr}) — Lot: <code>{adjustTarget.lotNr}</code>
            </p>
            <p style={{ fontSize: '0.875rem', marginBottom: '1rem' }}>
              Nåværende mengde: <strong>{adjustTarget.mengde} {adjustTarget.enhet}</strong>
            </p>
            <form onSubmit={handleAdjust}>
              {adjustError && <div className="alert alert-error" style={{ marginBottom: '0.75rem' }}>{adjustError}</div>}
              {adjustSuccess && <div className="alert alert-success" style={{ marginBottom: '0.75rem' }}>{adjustSuccess}</div>}
              <div className="form-group">
                <label>Ny mengde ({adjustTarget.enhet})</label>
                <input
                  type="number"
                  step="0.001"
                  min="0"
                  required
                  value={adjustMengde}
                  onChange={e => setAdjustMengde(e.target.value)}
                />
              </div>
              <div className="form-group">
                <label>Kommentar</label>
                <textarea
                  rows={2}
                  value={adjustKommentar}
                  onChange={e => setAdjustKommentar(e.target.value)}
                  placeholder="f.eks. Varetelling 2026-03-29"
                />
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setAdjustTarget(null)}>Avbryt</button>
                <button type="submit" className="btn btn-primary" disabled={adjustLoading}>
                  {adjustLoading ? 'Lagrer...' : 'Lagre'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {varetellingOpen && (
        <div className="modal-overlay" onClick={() => !varetellingSubmitting && setVaretellingOpen(false)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 700, maxHeight: '85vh', overflow: 'hidden', display: 'flex', flexDirection: 'column' }}>
            <h2>📦 Full varetelling</h2>
            {varetellingLoading ? (
              <p>Laster...</p>
            ) : varetellingOk ? (
              <div className="alert alert-success">Varetelling lagret!</div>
            ) : (
              <>
                <p style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '1rem' }}>
                  Fyll inn faktisk telling for hver artikkel. Avvik markeres automatisk.
                </p>
                <div style={{ overflowY: 'auto', flex: 1, border: '1px solid #e5e7eb', borderRadius: 8, marginBottom: '1rem' }}>
                  <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.875rem' }}>
                    <thead style={{ position: 'sticky', top: 0, background: '#f9fafb' }}>
                      <tr>
                        <th style={{ textAlign: 'left', padding: '0.5rem', borderBottom: '1px solid #e5e7eb' }}>Artikkelnr</th>
                        <th style={{ textAlign: 'left', padding: '0.5rem', borderBottom: '1px solid #e5e7eb' }}>Navn</th>
                        <th style={{ textAlign: 'right', padding: '0.5rem', borderBottom: '1px solid #e5e7eb' }}>System</th>
                        <th style={{ textAlign: 'left', padding: '0.5rem', borderBottom: '1px solid #e5e7eb' }}>Faktisk</th>
                        <th style={{ textAlign: 'right', padding: '0.5rem', borderBottom: '1px solid #e5e7eb' }}>Differanse</th>
                      </tr>
                    </thead>
                    <tbody>
                      {varetellingData.map(rad => (
                        <tr key={rad.artikkelId} style={{ borderBottom: '1px solid #f3f4f6' }}>
                          <td style={{ padding: '0.5rem' }}><code>{rad.artikkelNr}</code></td>
                          <td style={{ padding: '0.5rem' }}>{rad.artikkelNavn}</td>
                          <td style={{ padding: '0.5rem', textAlign: 'right', color: '#6b7280' }}>{rad.systemMengde}</td>
                          <td style={{ padding: '0.5rem' }}>
                            <input
                              type="number"
                              step="0.001"
                              min="0"
                              placeholder="0"
                              value={rad.faktiskMengde}
                              onChange={e => updateFaktiskMengde(rad.artikkelId, e.target.value)}
                              style={{ width: 100, padding: '4px 8px', border: '1px solid #d1d5db', borderRadius: 4 }}
                            />
                          </td>
                          <td style={{
                            padding: '0.5rem', textAlign: 'right',
                            color: rad.differanse > 0 ? '#16a34a' : rad.differanse < 0 ? '#dc2626' : '#9ca3af',
                            fontWeight: rad.differanse !== 0 ? 600 : 400,
                          }}>
                            {rad.differanse > 0 ? '+' : ''}{rad.differanse}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
                {varetellingError && <div className="alert alert-error" style={{ marginBottom: '1rem' }}>{varetellingError}</div>}
                {!varetellingConfirm ? (
                  <div className="form-actions">
                    <button type="button" className="btn btn-secondary" onClick={() => setVaretellingOpen(false)}>Avbryt</button>
                    <button
                      type="button"
                      className="btn btn-primary"
                      onClick={() => setVaretellingConfirm(true)}
                      disabled={varetellingData.every(r => r.faktiskMengde === '')}
                    >
                      Bekreft varetelling
                    </button>
                  </div>
                ) : (
                  <div style={{ background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: 8, padding: '1rem', marginBottom: '1rem' }}>
                    <p style={{ fontWeight: 600, marginBottom: '0.5rem' }}>Er du sikker?</p>
                    <p style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '1rem' }}>
                      Dette vil justere alle artikler med differanse. Handlingen logges med kommentar "Varetelling [dato]".
                    </p>
                    <div className="form-actions">
                      <button
                        type="button"
                        className="btn btn-secondary"
                        onClick={() => setVaretellingConfirm(false)}
                        disabled={varetellingSubmitting}
                      >Tilbake</button>
                      <button
                        type="button"
                        className="btn btn-primary"
                        onClick={submitVaretelling}
                        disabled={varetellingSubmitting}
                      >
                        {varetellingSubmitting ? 'Lagrer...' : 'Ja, lagre varetelling'}
                      </button>
                    </div>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      )}
    </>
  );
}
