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

  // Ny modal for artikkeldetaljer
  const [selectedArticle, setSelectedArticle] = useState<LagerBeholdning | null>(null);
  const [traceData, setTraceData] = useState<any>(null);
  const [traceLoading, setTraceLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<'lot' | 'historikk'>('lot');
  const [justerLot, setJusterLot] = useState<any>(null);
  const [justerNyMengde, setJusterNyMengde] = useState('');
  const [justerKommentar, setJusterKommentar] = useState('');
  const [justerError, setJusterError] = useState('');
  const [justerLoading, setJusterLoading] = useState(false);
  const [brukerErAdmin, setBrukerErAdmin] = useState(false);

  useEffect(() => {
    async function init() {
      try {
        const me = await getMe();
        setBrukerErAdmin(me.erAdmin);
      } catch {
        // Ignorer
      }
      loadData();
    }
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

  async function openArticleModal(artikkel: LagerBeholdning) {
    setSelectedArticle(artikkel);
    setTraceData(null);
    setActiveTab('lot');
    setTraceLoading(true);
    try {
      const data = await get(`/traceability/artikkel/${artikkel.artikkelId}`);
      setTraceData(data);
    } catch (err) {
      console.error('Kunne ikke hente sporbarhet:', err);
    } finally {
      setTraceLoading(false);
    }
  }

  function closeArticleModal() {
    setSelectedArticle(null);
    setTraceData(null);
    setJusterLot(null);
  }

  function prepareJuster(lot: any) {
    setJusterLot(lot);
    setJusterNyMengde(lot.gjeldendeMengde.toString());
    setJusterKommentar('');
    setJusterError('');
  }

  async function utførJuster() {
    if (!selectedArticle || !justerLot) return;
    setJusterLoading(true);
    setJusterError('');
    try {
      const success = await patch('/inventory/juster', {
        artikkelId: selectedArticle.artikkelId,
        lotNr: justerLot.lotNr,
        nyMengde: parseFloat(justerNyMengde),
        kommentar: justerKommentar || null,
      });
      if (success) {
        setJusterLot(null);
        // Oppdater sporbarhetsdata
        const data = await get(`/traceability/artikkel/${selectedArticle.artikkelId}`);
        setTraceData(data);
        await loadData();
      } else {
        setJusterError('Kunne ikke lagre justering.');
      }
    } catch (err) {
      setJusterError((err as Error).message);
    } finally {
      setJusterLoading(false);
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
                  onClick={() => openArticleModal(a.lots[0])}
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

      {/* Modal for artikkeldetaljer */}
      {selectedArticle && (
        <div className="modal-overlay" onClick={closeArticleModal}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 900, maxHeight: '85vh', overflow: 'hidden', display: 'flex', flexDirection: 'column' }}>
            <div className="modal-header position-relative">
              <div>
                <h4 className="mb-0">{selectedArticle.artikkelNavn}</h4>
                <small className="text-muted">Artikkelnr: {selectedArticle.artikkelNr}</small>
              </div>
              <button 
                className="btn btn-sm btn-outline-danger position-absolute" 
                onClick={closeArticleModal}
                style={{ 
                  top: '0.5rem', 
                  right: '0.5rem', 
                  width: '32px', 
                  height: '32px', 
                  display: 'flex', 
                  alignItems: 'center', 
                  justifyContent: 'center',
                  padding: 0,
                  zIndex: 10
                }}
              >
                ×
              </button>
            </div>
            <div className="modal-body" style={{ flex: 1, overflow: 'auto' }}>
              <div className="tab-nav mb-4">
                <div className="nav nav-tabs">
                  <button
                    className={`nav-link ${activeTab === 'lot' ? 'active' : ''}`}
                    onClick={() => setActiveTab('lot')}
                    style={{ fontWeight: 500, padding: '0.5rem 1rem' }}
                  >
                    📦 Lot-oversikt
                  </button>
                  <button
                    className={`nav-link ${activeTab === 'historikk' ? 'active' : ''}`}
                    onClick={() => setActiveTab('historikk')}
                    style={{ fontWeight: 500, padding: '0.5rem 1rem' }}
                  >
                    📊 Historikk
                  </button>
                </div>
              </div>

              {traceLoading ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Laster...</span>
                  </div>
                  <p className="mt-2 text-muted">Laster detaljer...</p>
                </div>
              ) : activeTab === 'lot' ? (
                <div>
                  <div className="d-flex justify-content-between align-items-center mb-3">
                    <h4 className="mb-0">Lot-oversikt</h4>
                    {brukerErAdmin && (
                      <div className="btn-group">
                        <button className="btn btn-sm btn-outline-primary">
                          📝 Rediger artikkel
                        </button>
                        <button className="btn btn-sm btn-outline-success">
                          ➕ Ny lot
                        </button>
                      </div>
                    )}
                  </div>
                  
                  <div className="table-responsive">
                    <table className="table table-hover table-sm">
                      <thead className="table-light">
                        <tr>
                          <th>LotNr</th>
                          <th>Beholdning</th>
                          <th>Enhet</th>
                          <th>Sist Oppdatert</th>
                          <th>Best før</th>
                          <th>Handlinger</th>
                        </tr>
                      </thead>
                      <tbody>
                        {(traceData?.lots ?? []).map((lot: any) => (
                          <tr key={lot.lotNr}>
                            <td>
                              <a href={`/sporing?lot=${lot.lotNr}`} target="_blank" className="text-decoration-none">
                                <code className="bg-light px-2 py-1 rounded">{lot.lotNr}</code>
                              </a>
                            </td>
                            <td>
                              <span className={`fw-bold ${lot.gjeldendeMengde < (lot.minBeholdning || 0) ? 'text-danger' : 'text-success'}`}>
                                {lot.gjeldendeMengde?.toFixed(2) || '0.00'}
                              </span>
                            </td>
                            <td>{lot.enhet}</td>
                            <td>{lot.sistOppdatert ? new Date(lot.sistOppdatert).toLocaleString('no-NO') : '-'}</td>
                            <td>
                              {lot.bestForDato ? (
                                <span className={`badge ${new Date(lot.bestForDato) < new Date() ? 'bg-danger' : 'bg-warning text-dark'}`}>
                                  {new Date(lot.bestForDato).toLocaleDateString('no-NO')}
                                </span>
                              ) : '-'}
                            </td>
                            <td>
                              <div className="btn-group btn-group-sm">
                                <button
                                  className="btn btn-outline-primary"
                                  onClick={() => prepareJuster(lot)}
                                  title="Juster beholdning"
                                >
                                  ✏️ Juster
                                </button>
                                <button
                                  className="btn btn-outline-info"
                                  onClick={() => window.open(`/sporing?lot=${lot.lotNr}`, '_blank')}
                                  title="Gå til sporing"
                                >
                                  🔍
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                  
                  <div className="alert alert-info mt-3">
                    <small>
                      <strong>Tips:</strong> Klikk på et lot-nummer for å gå til sporingssiden. 
                      {brukerErAdmin && 'Admin-brukere kan justere beholdning direkte fra denne tabellen.'}
                    </small>
                  </div>
                </div>
              ) : (
                <div>
                  <div className="d-flex justify-content-between align-items-center mb-3">
                    <h4 className="mb-0">Transaksjonshistorikk</h4>
                    <div className="btn-group btn-group-sm">
                      <button className="btn btn-outline-primary">📊 Eksporter</button>
                      <button className="btn btn-outline-info">🔍 Filter</button>
                      <button className="btn btn-outline-secondary">🔄 Oppdater</button>
                    </div>
                  </div>
                  
                  <div className="table-responsive">
                    <table className="table table-sm table-hover">
                      <thead className="table-light">
                        <tr>
                          <th>Tid</th>
                          <th>Lot</th>
                          <th>Hendelse</th>
                          <th>Etter</th>
                          <th>Bruker</th>
                          <th>Kommentar</th>
                        </tr>
                      </thead>
                      <tbody>
                        {(traceData?.lots ?? [])
                          .flatMap((lot: any) =>
                            (lot.transaksjoner || []).map((t: any) => ({
                              lot: lot.lotNr,
                              enhet: lot.enhet,
                              trans: t
                            }))
                          )
                          .sort((a: any, b: any) => new Date(b.trans.tidspunkt).getTime() - new Date(a.trans.tidspunkt).getTime())
                          .map(({ lot, enhet, trans }: any, idx: number) => (
                            <tr key={idx}>
                              <td>
                                <small className="text-muted">
                                  {new Date(trans.tidspunkt).toLocaleDateString('no-NO')}<br/>
                                  {new Date(trans.tidspunkt).toLocaleTimeString('no-NO', { hour: '2-digit', minute: '2-digit' })}
                                </small>
                              </td>
                              <td>
                                <code className="bg-light px-1 rounded">{lot}</code>
                              </td>
                              <td>
                                <div className="d-flex align-items-center">
                                  <span className={`badge ${trans.mengde > 0 ? 'bg-success' : 'bg-danger'} me-2`}>
                                    {trans.mengde > 0 ? '+' : ''}{trans.mengde.toFixed(2)}
                                  </span>
                                  <div>
                                    <div className="fw-medium">{trans.kilde}</div>
                                    <small className="text-muted">
                                      {trans.kildeId ? `ID: ${trans.kildeId}` : ''}
                                    </small>
                                  </div>
                                </div>
                              </td>
                              <td>
                                <span className="fw-bold">{trans.beholdningEtter?.toFixed(2) || '0.00'}</span>
                                <small className="d-block text-muted">{enhet}</small>
                              </td>
                              <td>
                                <span className="badge bg-secondary">{trans.utfortAv || 'System'}</span>
                              </td>
                              <td>
                                {trans.kommentar ? (
                                  <small className="text-muted" title={trans.kommentar}>
                                    {trans.kommentar.length > 30 ? trans.kommentar.substring(0, 30) + '...' : trans.kommentar}
                                  </small>
                                ) : '-'}
                              </td>
                            </tr>
                          ))
                        }
                      </tbody>
                    </table>
                  </div>
                  
                  <div className="alert alert-light mt-3">
                    <small>
                      <strong>Totalt:</strong> {(traceData?.lots ?? []).flatMap((l: any) => l.transaksjoner || []).length} transaksjoner
                    </small>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Modal for justering av lot */}
      {justerLot && selectedArticle && (
        <div className="modal-overlay" onClick={() => setJusterLot(null)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 500 }}>
            <div className="modal-header">
              <h4>Juster beholdning</h4>
              <button className="btn-close" onClick={() => setJusterLot(null)}>×</button>
            </div>
            <div className="modal-body">
              <div className="mb-3">
                <p className="mb-1">
                  <strong>{selectedArticle.artikkelNavn}</strong> ({selectedArticle.artikkelNr})
                </p>
                <p className="mb-0 text-muted">
                  Lot: <code>{justerLot.lotNr}</code> • Nåværende: {justerLot.gjeldendeMengde} {justerLot.enhet}
                </p>
              </div>
              
              {justerError && (
                <div className="alert alert-danger">{justerError}</div>
              )}
              
              <div className="mb-3">
                <label className="form-label">Ny mengde ({justerLot.enhet})</label>
                <input
                  type="number"
                  className="form-control"
                  step="0.001"
                  min="0"
                  value={justerNyMengde}
                  onChange={e => setJusterNyMengde(e.target.value)}
                  disabled={justerLoading}
                />
              </div>
              
              <div className="mb-3">
                <label className="form-label">Kommentar (valgfritt)</label>
                <textarea
                  className="form-control"
                  rows={2}
                  value={justerKommentar}
                  onChange={e => setJusterKommentar(e.target.value)}
                  disabled={justerLoading}
                  placeholder="F.eks. 'Korrigering etter varetelling'"
                />
              </div>
              
              <div className="d-flex justify-content-between">
                <button
                  className="btn btn-outline-secondary"
                  onClick={() => setJusterLot(null)}
                  disabled={justerLoading}
                >
                  Avbryt
                </button>
                <button
                  className="btn btn-primary"
                  onClick={utførJuster}
                  disabled={justerLoading}
                >
                  {justerLoading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-1"></span>
                      Lagrer...
                    </>
                  ) : 'Lagre justering'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

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
