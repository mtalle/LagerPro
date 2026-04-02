'use client';
import { useEffect, useState } from 'react';
import { LagerBeholdning, get, patch, getMe } from '../../lib/api';

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

  const filtered = beholdninger.filter(b => {
    const matchesSearch =
      b.artikkelNavn.toLowerCase().includes(search.toLowerCase()) ||
      b.artikkelNr.toLowerCase().includes(search.toLowerCase()) ||
      b.lotNr.toLowerCase().includes(search.toLowerCase()) ||
      (b.lokasjon ?? '').toLowerCase().includes(search.toLowerCase());

    const erLav = b.minBeholdning != null && b.mengde < b.minBeholdning;

    return matchesSearch && (!visKunLav || erLav);
  });

  if (loading) return <div className="loading">Laster lager...</div>;

  return (
    <>
      <div className="page-header">
        <h1>🏭 Lagerbeholdning</h1>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Søk artikkel, lotnr eller lokasjon..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <label style={{ display: 'flex', alignItems: 'center', gap: 6, fontSize: '0.9rem', cursor: 'pointer', userSelect: 'none' }}>
          <input type="checkbox" checked={visKunLav} onChange={e => setVisKunLav(e.target.checked)} />
          Vis kun lav beholdning
        </label>
        <span style={{ marginLeft: 'auto', fontSize: '0.85rem', color: '#6b7280' }}>{filtered.length} av {beholdninger.length}</span>
      </div>

      <table className="table-scroll">
        <thead>
          <tr>
            <th></th>
            <th>Artikkelnr</th>
            <th>Navn</th>
            <th>Lotnr</th>
            <th>Mengde</th>
            <th>Enhet</th>
            <th>Lokasjon</th>
            <th>Best-før</th>
            <th>Min</th>
            <th>Oppdatert</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={11} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen beholdning funnet</td></tr>
          ) : filtered.map(b => {
            const erLav = b.minBeholdning != null && b.mengde < b.minBeholdning;
            return (
              <tr key={b.id} style={erLav ? { background: '#fef2f2' } : undefined}>
                <td style={{ width: 32, textAlign: 'center' }}>
                  {erLav && <span title={`Lav beholdning — minst ${b.minBeholdning} ${b.enhet}`}>⚠️</span>}
                </td>
                <td><code>{b.artikkelNr}</code></td>
                <td>{b.artikkelNavn}</td>
                <td><code>{b.lotNr}</code></td>
                <td><strong style={erLav ? { color: '#dc2626' } : undefined}>{b.mengde}</strong></td>
                <td>{b.enhet}</td>
                <td>{b.lokasjon ? <span style={{ fontSize: '0.85rem', background: '#f3f4f6', padding: '1px 6px', borderRadius: 4 }}>{b.lokasjon}</span> : <span style={{ color: '#d1d5db' }}>—</span>}</td>
                <td>{b.bestForDato ? new Date(b.bestForDato).toLocaleDateString('no-NO') : '—'}</td>
                <td style={{ color: erLav ? '#dc2626' : erLav === false ? '#16a34a' : '#9ca3af', fontSize: '0.85rem' }}>
                  {b.minBeholdning != null ? b.minBeholdning : '—'}
                </td>
                <td style={{ fontSize: '0.85rem', color: '#6b7280' }}>{new Date(b.sistOppdatert).toLocaleDateString('no-NO')}</td>
                <td>
                  <button className="btn btn-sm btn-secondary" onClick={() => openAdjust(b)}>Juster</button>
                </td>
              </tr>
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
    </>
  );
}
