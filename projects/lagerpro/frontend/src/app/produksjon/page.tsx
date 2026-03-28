'use client';
import { useEffect, useState } from 'react';
import { ProduksjonsOrdre, Resept, get, post, patch } from '../../lib/api';

const STATUS_MAP: Record<string, string> = {
  Planlagt: 'badge-planlagt',
  IProduksjon: 'badge-i-produksjon',
  Ferdigmeldt: 'badge-ferdigmeldt',
  Kansellert: 'badge-kansellert',
};

export default function ProduksjonPage() {
  const [ordre, setOrdre] = useState<ProduksjonsOrdre[]>([]);
  const [resepter, setResepter] = useState<Resept[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);

  const [form, setForm] = useState({
    reseptId: 0, ordreNr: '', planlagtDato: new Date().toISOString().slice(0, 10),
    kommentar: '',
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

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (form.reseptId === 0) { alert('Velg en resept.'); return; }
    try {
      await post('/production', {
        ...form,
        planlagtDato: new Date(form.planlagtDato).toISOString(),
      });
      setShowModal(false);
      resetForm();
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  function resetForm() {
    setForm({ reseptId: 0, ordreNr: '', planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/production/${id}/status`, { status }); load(); }
    catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  if (loading) return <div className="loading">Laster produksjonsordrer...</div>;

  return (
    <>
      <div className="page-header">
        <h1>🏗 Produksjon</h1>
        <button className="btn btn-primary" onClick={() => setShowModal(true)}>+ Ny produksjonsordre</button>
      </div>

      <table>
        <thead>
          <tr><th>OrdreNr</th><th>Resept</th><th>Planlagt</th><th>Ferdigmeldt</th><th>Antall</th><th>Lot</th><th>Status</th><th>Utfort</th><th></th></tr>
        </thead>
        <tbody>
          {ordre.length === 0 ? (
            <tr><td colSpan={9} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen produksjonsordrer</td></tr>
          ) : ordre.map(o => (
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
                {o.status === 'IProduksjon' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(o.id, 'Ferdigmeldt')}>Ferdigmeld</button>}
                {o.status !== 'Ferdigmeldt' && o.status !== 'Kansellert' && <button className="btn btn-sm btn-danger" onClick={() => setStatus(o.id, 'Kansellert')}>Kanseller</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 500 }}>
            <h2>Ny produksjonsordre</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Resept *</label>
                  <select required value={form.reseptId} onChange={e => setForm({ ...form, reseptId: parseInt(e.target.value) })}>
                    <option value={0}>Velg resept...</option>
                    {resepter.map(r => <option key={r.id} value={r.id}>{r.navn} ({r.antallPortjoner} porsjoner)</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Ordrenummer</label>
                  <input value={form.ordreNr} onChange={e => setForm({ ...form, ordreNr: e.target.value })} placeholder="Auto hvis tom" />
                </div>
                <div className="form-group">
                  <label>Planlagt dato</label>
                  <input type="date" value={form.planlagtDato} onChange={e => setForm({ ...form, planlagtDato: e.target.value })} />
                </div>
              </div>
              <div className="form-group">
                <label>Kommentar</label>
                <textarea rows={2} value={form.kommentar} onChange={e => setForm({ ...form, kommentar: e.target.value })} />
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => { setShowModal(false); resetForm(); }}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Opprett</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
