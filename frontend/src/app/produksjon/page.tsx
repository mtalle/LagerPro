'use client';
import { useEffect, useState } from 'react';
import { ProduksjonsOrdre, Resept, get, post, patch } from '../../lib/api';

const STATUS_MAP: Record<string, string> = {
  Planlagt: 'badge-planlagt',
  IProduksjon: 'badge-i-produksjon',
  Ferdigmeldt: 'badge-ferdigmeldt',
  Kansellert: 'badge-kansellert',
};

interface FerdigmeldLinje {
  ravareId: number;
  ravareNavn: string | null;
  enhet: string | null;
  oppskriftsMengde: number;
  foreslattLotNr: string | null;
  foreslattMengde: number | null;
  tilgjengeligBeholdning: number | null;
  harLager: boolean;
}

interface FerdigmeldPrefill {
  ordreId: number;
  ordreNr: string;
  reseptId: number;
  reseptNavn: string | null;
  ferdigvareId: number;
  ferdigvareNavn: string | null;
  antallPortjoner: number;
  ferdigvareEnhet: string | null;
  foreslattAntall: number;
  reseptLinjer: FerdigmeldLinje[];
}

export default function ProduksjonPage() {
  const [ordre, setOrdre] = useState<ProduksjonsOrdre[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showFerdigmeldModal, setShowFerdigmeldModal] = useState(false);
  const [ferdigmeldPrefill, setFerdigmeldPrefill] = useState<FerdigmeldPrefill | null>(null);
  const [search, setSearch] = useState('');

  const [resepter, setResepter] = useState<Resept[]>([]);

  const [createForm, setCreateForm] = useState({ reseptId: 0, planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });

  const [ferdigmeldForm, setFerdigmeldForm] = useState({
    antallProdusert: 1,
    kommentar: '',
    utfortAv: '',
    forbruk: [] as { artikkelId: number; lotNr: string; mengdeBrukt: number; enhet: string; overstyrt: boolean; kommentar: string }[],
  });

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const [o, r] = await Promise.all([
        get<ProduksjonsOrdre[]>('/production'),
        get<Resept[]>('/recipes'),
      ]);
      setOrdre(o);
      setResepter(r);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    if (!createForm.reseptId) { alert('Velg en resept.'); return; }
    try {
      await post('/production', { ...createForm, planlagtDato: new Date(createForm.planlagtDato).toISOString() });
      setShowCreateModal(false);
      setCreateForm({ reseptId: 0, planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
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
      alert('Kunne ikke laste ferdigmeld-data: ' + (e as Error).message);
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
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/production/${id}/status`, { status }); load(); }
    catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  if (loading) return <div className="loading">Laster produksjonsordrer...</div>;

  const filtered = ordre.filter(o => {
    const q = search.toLowerCase();
    return (
      o.ordreNr.toLowerCase().includes(q) ||
      (o.reseptNavn ?? '').toLowerCase().includes(q) ||
      (o.utfortAv ?? '').toLowerCase().includes(q)
    );
  });

  return (
    <>
      <div className="page-header">
        <h1>🏗 Produksjon</h1>
        <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}>+ Ny produksjonsordre</button>
      </div>

      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', marginBottom: '1rem' }}>
        <input
          placeholder="Søk ordrenr, resept..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 300, fontSize: '0.9rem' }}
        />
      </div>

      <table>
        <thead>
          <tr><th>OrdreNr</th><th>Resept</th><th>Planlagt</th><th>Ferdigmeldt</th><th>Antall</th><th>Lot</th><th>Status</th><th>Utfort</th><th></th></tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={9} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>{ordre.length === 0 ? 'Ingen produksjonsordrer' : 'Ingen resultater'}</td></tr>
          ) : filtered.map(o => (
            <tr key={o.id}>
              <td><code>{o.ordreNr}</code></td>
              <td>{o.reseptNavn ?? `Resept.ID ${o.reseptId}`}</td>
              <td>{new Date(o.planlagtDato).toLocaleDateString('no-NO')}</td>
              <td>{o.ferdigmeldtDato ? new Date(o.ferdigmeldtDato).toLocaleDateString('no-NO') : '—'}</td>
              <td>{o.antallProdusert}</td>
              <td><code>{o.ferdigvareLotNr}</code></td>
              <td><span className={`badge ${STATUS_MAP[o.status] ?? ''}`}>{o.status}</span></td>
              <td>{o.utfortAv ?? '—'}</td>
              <td>
                {o.status === 'Planlagt' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(o.id, 'IProduksjon')}>Start</button>}
                {o.status === 'IProduksjon' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => openFerdigmeld(o)}>Ferdigmeld</button>}
                {o.status !== 'Ferdigmeldt' && o.status !== 'Kansellert' && <button className="btn btn-sm btn-danger" onClick={() => setStatus(o.id, 'Kansellert')}>Kanseller</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showCreateModal && (
        <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>Ny produksjonsordre</h2>
            <form onSubmit={handleCreate}>
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
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => { setShowFerdigmeldModal(false); setFerdigmeldPrefill(null); }}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Ferdigmeld</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
