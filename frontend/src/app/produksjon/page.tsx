'use client';
import { useEffect, useState, Fragment } from 'react';
import { ProduksjonsOrdre, Resept, Plukkliste, PlukklisteLinje, FerdigmeldPrefill, FerdigmeldLinje, get, post, patch, del, getMe } from '../../lib/api';
import { useTilgang } from '../../lib/useTilgang';

const STATUS_MAP: Record<string, string> = {
  Planlagt: 'badge-planlagt',
  IProduksjon: 'badge-i-produksjon',
  Ferdigmeldt: 'badge-ferdigmeldt',
  Kansellert: 'badge-kansellert',
};

export default function ProduksjonPage() {
  const [ordre, setOrdre] = useState<ProduksjonsOrdre[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showFerdigmeldModal, setShowFerdigmeldModal] = useState(false);
  const [expandedOrdreId, setExpandedOrdreId] = useState<number | null>(null);
  const [showPlukklisteModal, setShowPlukklisteModal] = useState(false);
  const [plukkliste, setPlukkliste] = useState<Plukkliste | null>(null);
  const [plukklisteOrdreId, setPlukklisteOrdreId] = useState<number | null>(null);
  const [plukklisteLoading, setPlukklisteLoading] = useState(false);
  const [ferdigmeldPrefill, setFerdigmeldPrefill] = useState<FerdigmeldPrefill | null>(null);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [error, setError] = useState('');
  const kanRedigere = useTilgang(4);

  const [resepter, setResepter] = useState<Resept[]>([]);

  const [createForm, setCreateForm] = useState({ reseptId: 0, planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });

  const [ferdigmeldForm, setFerdigmeldForm] = useState({
    antallProdusert: 1,
    kommentar: '',
    utfortAv: '',
    bekreftLagring: false,
    forbruk: [] as { artikkelId: number; lotNr: string; mengdeBrukt: number; enhet: string; overstyrt: boolean; kommentar: string }[],
  });

  useEffect(() => {
    async function init() { try { await getMe(); } catch { return; } load(); }
    init();
  }, []);

  async function load() {
    try {
      const [o, r] = await Promise.all([
        get<ProduksjonsOrdre[]>('/production'),
        get<Resept[]>('/recipes'),
      ]);
      setOrdre(o);
      setResepter(r);
    } catch (e) { setError((e as Error).message); }
    finally { setLoading(false); }
  }

  async function openPlukkliste(ordreId?: number) {
    setPlukklisteOrdreId(ordreId ?? null);
    setShowPlukklisteModal(true);
    if (ordreId !== undefined && plukkliste && plukklisteOrdreId === ordreId) return; // already loaded for this order
    if (ordreId === undefined && plukkliste && plukklisteOrdreId === null) return; // already loaded global
    setPlukklisteLoading(true);
    setPlukkliste(null);
    try {
      const url = ordreId !== undefined ? `/production/${ordreId}/plukkliste` : '/production/plukkliste';
      const data = await get<Plukkliste>(url);
      setPlukkliste(data);
    } catch (e) { setError((e as Error).message); }
    finally { setPlukklisteLoading(false); }
  }

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    if (!createForm.reseptId) { setError('Velg en resept.'); return; }
    setError('');
    try {
      await post('/production', { ...createForm, planlagtDato: new Date(createForm.planlagtDato).toISOString() });
      setShowCreateModal(false);
      setCreateForm({ reseptId: 0, planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });
      load();
    } catch (e) { setError('Feil: ' + (e as Error).message); }
  }

  async function openFerdigmeld(o: ProduksjonsOrdre) {
    try {
      const prefill = await get<FerdigmeldPrefill>(`/production/${o.id}/ferdigmeld/prefill`);
      setFerdigmeldPrefill(prefill);
      const scale = prefill.foreslattAntall > 0 ? prefill.foreslattAntall : 1;
      setFerdigmeldForm({
        antallProdusert: scale,
        kommentar: '',
        utfortAv: '',
        bekreftLagring: false,
        forbruk: prefill.reseptLinjer.map(linje => ({
          artikkelId: linje.ravareId,
          lotNr: linje.foreslattLotNr ?? '',
          mengdeBrukt: Math.round(linje.oppskriftsMengde * scale * 100) / 100,
          enhet: linje.enhet ?? 'STK',
          overstyrt: false,
          kommentar: '',
        })),
      });
      setShowFerdigmeldModal(true);
    } catch (e) {
      setError('Kunne ikke laste ferdigmeld-data: ' + (e as Error).message);
    }
  }

  function updateForbrukLinje(i: number, field: string, value: unknown) {
    const forbruk = [...ferdigmeldForm.forbruk];
    (forbruk[i] as Record<string, unknown>)[field] = value;
    setFerdigmeldForm({ ...ferdigmeldForm, forbruk });
  }

  async function handleFerdigmeld(e: React.FormEvent) {
    e.preventDefault();
    if (!ferdigmeldPrefill) return;
    if (!ferdigmeldForm.bekreftLagring) {
      setError('Du må bekrefte før lagring.');
      return;
    }
    try {
      await post(`/production/${ferdigmeldPrefill.ordreId}/ferdigmeld`, {
        antallProdusert: ferdigmeldForm.antallProdusert,
        kommentar: ferdigmeldForm.kommentar || null,
        utfortAv: ferdigmeldForm.utfortAv || null,
        forbruk: ferdigmeldForm.forbruk.map(f => ({
          artikkelId: f.artikkelId,
          lotNr: f.lotNr || null,
          mengdeBrukt: f.mengdeBrukt,
          enhet: f.enhet || null,
          overstyrt: f.overstyrt,
          kommentar: f.kommentar || null,
        })),
      });
      setShowFerdigmeldModal(false);
      setFerdigmeldPrefill(null);
      setFerdigmeldForm({ ...ferdigmeldForm, bekreftLagring: false });
      load();
    } catch (e) { setError('Feil: ' + (e as Error).message); }
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/production/${id}/status`, { status }); load(); }
    catch (e) { setError('Feil: ' + (e as Error).message); }
  }

  async function handleDelete(id: number) {
    if (!confirm('Er du sikker på at du vil slette denne produksjonsordren?')) return;
    try { await del(`/production/${id}`); load(); }
    catch (e) { setError('Feil: ' + (e as Error).message); }
  }

  if (loading) return <div className="loading">Laster produksjonsordrer...</div>;

  const filtered = ordre.filter(o => {
    const q = search.toLowerCase();
    const matchesSearch =
      o.ordreNr.toLowerCase().includes(q) ||
      (o.reseptNavn ?? '').toLowerCase().includes(q) ||
      (o.utfortAv ?? '').toLowerCase().includes(q);
    const matchesStatus = !statusFilter || o.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  return (
    <>
      <div className="page-header">
        <h1>🏗 Produksjon</h1>
        <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}>+ Ny produksjonsordre</button>
      </div>

      <div className="filter-bar">
        <input className="search-input" placeholder="Søk ordrenr, resept..." value={search} onChange={e => setSearch(e.target.value)} />
        <div className="status-filter">
          {['', 'Planlagt', 'IProduksjon', 'Ferdigmeldt', 'Kansellert'].map(s => (
            <button key={s} type="button"
              onClick={() => setStatusFilter(s)}
              className={statusFilter === s ? 'btn btn-primary btn-sm' : 'btn btn-secondary btn-sm'}>
              {s || 'Alle'}
            </button>
          ))}
        </div>
        <span className="filter-count">{filtered.length} av {ordre.length}</span>
      </div>

      <div className="table-wrapper">
      <table className="table-scroll">
        <thead>
          <tr><th>OrdreNr</th><th>Resept</th><th>Ferdigvare</th><th>Planlagt</th><th>Ferdigmeldt</th><th>Antall</th><th>Lot</th><th>Status</th><th>Utfort</th></tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={9} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>{ordre.length === 0 ? 'Ingen produksjonsordrer' : 'Ingen resultater'}</td></tr>
          ) : filtered.map(o => (
            <Fragment key={o.id}>
              <tr className={expandedOrdreId === o.id ? 'expanded-row' : ''}
                onClick={() => setExpandedOrdreId(expandedOrdreId === o.id ? null : o.id)}
                style={{ cursor: 'pointer' }}>
                <td><code>{o.ordreNr}</code></td>
                <td>{o.reseptNavn ?? `Resept.ID ${o.reseptId}`}</td>
                <td>{o.ferdigvareNavn ?? '—'}{o.ferdigvareEnhet ? ` (${o.ferdigvareEnhet})` : ''}</td>
                <td>{new Date(o.planlagtDato).toLocaleDateString('no-NO')}</td>
                <td>{o.ferdigmeldtDato ? new Date(o.ferdigmeldtDato).toLocaleDateString('no-NO') : '—'}</td>
                <td>{o.antallProdusert}</td>
                <td><code>{o.ferdigvareLotNr}</code></td>
                <td><span className={`badge ${STATUS_MAP[o.status] ?? ''}`}>{o.status}</span></td>
                <td>{o.utfortAv ?? '—'}</td>
              </tr>
              {expandedOrdreId === o.id && (
                <tr key={`${o.id}-expanded`} className="expanded-actions-row">
                  <td colSpan={9} style={{ padding: '0.75rem 1rem', background: '#f9fafb', borderTop: '1px solid #e5e7eb' }}>
                    <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap', alignItems: 'center' }}>
                      {kanRedigere && o.status === 'Planlagt' && (
                        <button className="btn btn-sm btn-primary" onClick={(e) => { e.stopPropagation(); setStatus(o.id, 'IProduksjon'); setExpandedOrdreId(null); }}>Start</button>
                      )}
                      {kanRedigere && o.status === 'IProduksjon' && (
                        <button className="btn btn-sm btn-primary" onClick={(e) => { e.stopPropagation(); openFerdigmeld(o); }}>Ferdigmeld</button>
                      )}
                      {(o.status === 'IProduksjon' || o.status === 'Ferdigmeldt') && (
                        <button className="btn btn-sm btn-secondary" onClick={(e) => { e.stopPropagation(); openPlukkliste(o.id); }}>📋 Plukkliste</button>
                      )}
                      {kanRedigere && o.status === 'Planlagt' && (
                        <button className="btn btn-sm btn-danger" onClick={(e) => { e.stopPropagation(); handleDelete(o.id); }}>Slett</button>
                      )}
                      {kanRedigere && o.status !== 'Ferdigmeldt' && o.status !== 'Kansellert' && (
                        <button className="btn btn-sm btn-danger" onClick={(e) => { e.stopPropagation(); setStatus(o.id, 'Kansellert'); }}>Kanseller</button>
                      )}
                      {kanRedigere && (o.status === 'Planlagt' || o.status === 'IProduksjon') && (
                        <span style={{ marginLeft: 'auto', fontSize: '0.8rem', color: '#6b7280' }}>
                          Planlagt mengde: <strong>{o.antallProdusert}</strong> {o.ferdigvareEnhet ?? 'STK'}
                        </span>
                      )}
                    </div>
                  </td>
                </tr>
              )}
            </Fragment>
          ))}
        </tbody>
      </table>
      </div>

      {showCreateModal && (
        <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>Ny produksjonsordre</h2>
            <form onSubmit={handleCreate}>
              {error && <div className="alert alert-error">{error}</div>}
              <div className="form-grid">
                <div className="form-group">
                  <label>Resept *</label>
                  <select required value={createForm.reseptId} onChange={e => setCreateForm({ ...createForm, reseptId: parseInt(e.target.value) || 0 })}>
                    <option value={0}>Velg resept...</option>
                    {resepter.filter(r => r.aktiv).map(r => <option key={r.id} value={r.id}>{r.navn}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Planlagt dato *</label>
                  <input type="date" required value={createForm.planlagtDato} onChange={e => setCreateForm({ ...createForm, planlagtDato: e.target.value })} />
                </div>
              </div>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Kommentar</label>
                <textarea rows={2} value={createForm.kommentar} onChange={e => setCreateForm({ ...createForm, kommentar: e.target.value })} />
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowCreateModal(false)}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Opprett ordre</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showFerdigmeldModal && ferdigmeldPrefill && (
        <div className="modal-overlay" onClick={() => { setShowFerdigmeldModal(false); setFerdigmeldPrefill(null); }}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 850 }}>
            <h2>Ferdigmeld produksjon #{ferdigmeldPrefill.ordreNr}</h2>
            <p style={{ color: '#6b7280', fontSize: '0.9rem', marginBottom: '1rem' }}>
              {ferdigmeldPrefill.reseptNavn} — {ferdigmeldPrefill.ferdigvareNavn ?? '—'}
            </p>
            <form onSubmit={handleFerdigmeld}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Planlagt mengde</label>
                  <input type="number" disabled value={ferdigmeldPrefill.foreslattAntall} style={{ background: '#f3f4f6', color: '#6b7280' }} />
                  <small style={{ color: '#9ca3af', fontSize: '0.75rem' }}>Planlagt mengde fra ordren</small>
                </div>
                <div className="form-group">
                  <label>Antall produsert *</label>
                  <input type="number" required min="0.01" step="0.01"
                    value={ferdigmeldForm.antallProdusert}
                    onChange={e => {
                      const nytt = parseFloat(e.target.value) || 0;
                      const base = ferdigmeldPrefill.foreslattAntall > 0 ? ferdigmeldPrefill.foreslattAntall : 1;
                      const scale = nytt / base;
                      setFerdigmeldForm({
                        ...ferdigmeldForm,
                        antallProdusert: nytt,
                        forbruk: ferdigmeldPrefill.reseptLinjer.map(linje => ({
                          artikkelId: linje.ravareId,
                          lotNr: linje.foreslattLotNr ?? '',
                          mengdeBrukt: Math.round(linje.oppskriftsMengde * scale * 100) / 100,
                          enhet: linje.enhet ?? 'STK',
                          overstyrt: false,
                          kommentar: '',
                        })),
                      });
                    }}
                  />
                </div>
                <div className="form-group">
                  <label>Utfort av</label>
                  <input value={ferdigmeldForm.utfortAv}
                    onChange={e => setFerdigmeldForm({ ...ferdigmeldForm, utfortAv: e.target.value })} />
                </div>
              </div>

              <h3 style={{ fontSize: '0.95rem', marginBottom: '0.5rem', color: '#374151' }}>Forbruk</h3>
              <div style={{ overflowX: 'auto', marginBottom: '1rem' }}>
                <table style={{ width: '100%', fontSize: '0.85rem' }}>
                  <thead>
                    <tr style={{ textAlign: 'left', background: '#f3f4f6' }}>
                      <th>Råvare</th><th>Oppskrift</th><th>LotNr</th><th>Mengde</th><th>Enhet</th><th>Lager</th>
                    </tr>
                  </thead>
                  <tbody>
                    {ferdigmeldPrefill.reseptLinjer.map((linje, i) => (
                      <tr key={linje.ravareId}>
                        <td>{linje.ravareNavn ?? `ID ${linje.ravareId}`}</td>
                        <td>{linje.oppskriftsMengde} {linje.enhet ?? ''}</td>
                        <td>
                          <input value={ferdigmeldForm.forbruk[i]?.lotNr ?? ''}
                            onChange={e => updateForbrukLinje(i, 'lotNr', e.target.value)}
                            placeholder="Lotnr"
                            style={{ width: 100, fontSize: '0.8rem', padding: '0.25rem 0.4rem' }} />
                        </td>
                        <td>
                          <input type="number" step="0.01" min="0"
                            value={ferdigmeldForm.forbruk[i]?.mengdeBrukt ?? 0}
                            onChange={e => updateForbrukLinje(i, 'mengdeBrukt', parseFloat(e.target.value) || 0)}
                            style={{ width: 80, fontSize: '0.8rem', padding: '0.25rem 0.4rem' }} />
                        </td>
                        <td>
                          <select value={ferdigmeldForm.forbruk[i]?.enhet ?? 'STK'}
                            onChange={e => updateForbrukLinje(i, 'enhet', e.target.value)}
                            style={{ fontSize: '0.8rem' }}>
                            <option value="KG">KG</option>
                            <option value="L">L</option>
                            <option value="STK">STK</option>
                            <option value="M">M</option>
                          </select>
                        </td>
                        <td>
                          <span style={{ fontSize: '0.8rem', color: linje.harLager ? '#16a34a' : '#dc2626' }}>
                            {linje.tilgjengeligBeholdning != null ? `${linje.tilgjengeligBeholdning} ${linje.enhet ?? ''}` : '—'}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Kommentar</label>
                <textarea rows={2} value={ferdigmeldForm.kommentar}
                  onChange={e => setFerdigmeldForm({ ...ferdigmeldForm, kommentar: e.target.value })} />
              </div>
              {error && <div className="alert alert-error" style={{ marginBottom: '1rem' }}>{error}</div>}
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                  <input type="checkbox" checked={ferdigmeldForm.bekreftLagring}
                    onChange={e => { setFerdigmeldForm({ ...ferdigmeldForm, bekreftLagring: e.target.checked }); setError(''); }} />
                  <span> Bekreft ferdigmelding av {ferdigmeldForm.antallProdusert} {ferdigmeldPrefill.ferdigvareEnhet ?? 'STK'}</span>
                </label>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => { setShowFerdigmeldModal(false); setFerdigmeldPrefill(null); }}>Avbryt</button>
                <button type="submit" className="btn btn-primary" disabled={!ferdigmeldForm.bekreftLagring}>Ferdigmeld</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showPlukklisteModal && (
        <div className="modal-overlay" onClick={() => { setShowPlukklisteModal(false); setPlukkliste(null); setPlukklisteOrdreId(null); }}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 900 }}>
            <h2>📋 Plukkliste</h2>
            {plukklisteLoading ? (
              <div className="loading">Laster plukkliste...</div>
            ) : plukkliste && plukkliste.linjer.length > 0 ? (
              <>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', marginBottom: '1rem' }}>
                  {plukklisteOrdreId ? `Plukkliste for ordre #${plukklisteOrdreId}` : 'Alle aktive produksjonsordrer'} — {plukkliste.linjer.length} linjer
                </p>
                <div style={{ overflowX: 'auto' }}>
                  <table style={{ width: '100%', fontSize: '0.85rem' }}>
                    <thead>
                      <tr style={{ background: '#f3f4f6', textAlign: 'left' }}>
                        <th>OrdreNr</th><th>Resept</th><th>Ferdigvare</th><th>Råvare</th><th>LOT</th><th>Skal plukkes</th><th>Tilgjengelig</th><th>Enhet</th><th>Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {plukkliste.linjer.map((linje, i) => (
                        <tr key={i} style={linje.lotNr === '-' ? { background: '#fef2f2' } : undefined}>
                          <td><code>{linje.ordreNr}</code></td>
                          <td>{linje.reseptNavn ?? `Resept.ID ${linje.reseptId}`}</td>
                          <td>{linje.ferdigvareNavn}</td>
                          <td>{linje.ravareNavn ?? `Råvare.ID ${linje.ravareId}`}</td>
                          <td><code style={{ color: linje.lotNr === '-' ? '#dc2626' : undefined }}>{linje.lotNr}</code></td>
                          <td><strong>{linje.feltAntall != null ? linje.feltAntall.toFixed(2) : '—'}</strong></td>
                          <td>{linje.lotNr === '-' ? <span style={{ color: '#dc2626' }}>MANGLER</span> : linje.mengde.toFixed(2)}</td>
                          <td>{linje.enhet}</td>
                          <td><span className="badge badge-planlagt">{linje.status}</span></td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </>
            ) : (
              <p style={{ color: '#6b7280', textAlign: 'center', padding: '2rem' }}>
                Ingen aktive produksjonsordrer med reseptlinjer.
              </p>
            )}
            <div className="form-actions" style={{ marginTop: '1rem' }}>
              <button type="button" className="btn btn-secondary" onClick={() => setShowPlukklisteModal(false)}>Lukk</button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
