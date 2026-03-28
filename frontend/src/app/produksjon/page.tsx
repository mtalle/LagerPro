'use client';
import { useEffect, useState } from 'react';
import { ProduksjonsOrdre, Resept, LagerBeholdning, get, post, patch } from '../../lib/api';

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
  const [ferdigmeldOrdre, setFerdigmeldOrdre] = useState<ProduksjonsOrdre | null>(null);

  const [resepter, setResepter] = useState<Resept[]>([]);

  const [createForm, setCreateForm] = useState({ reseptId: 0, planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });
  const [ferdigmeldForm, setFerdigmeldForm] = useState({ antallProdusert: 1, kommentar: '', utfortAv: '', forbruk: [] as { artikkelId: number; lotNr: string; mengdeBrukt: number; enhet: string; overstyrt: boolean; kommentar: string }[] });

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
    try {
      await post('/production', { ...createForm, planlagtDato: new Date(createForm.planlagtDato).toISOString() });
      setShowCreateModal(false);
      setCreateForm({ reseptId: 0, planlagtDato: new Date().toISOString().slice(0, 10), kommentar: '' });
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  function openFerdigmeld(o: ProduksjonsOrdre) {
    setFerdigmeldOrdre(o);
    // Pre-fill from order
    setFerdigmeldForm({ antallProdusert: 1, kommentar: '', utfortAv: '', forbruk: [] });
    setShowFerdigmeldModal(true);
  }

  async function handleFerdigmeld(e: React.FormEvent) {
    e.preventDefault();
    if (!ferdigmeldOrdre) return;
    try {
      await post(`/production/${ferdigmeldOrdre.id}/ferdigmeld`, {
        antallProdusert: ferdigmeldForm.antallProdusert,
        kommentar: ferdigmeldForm.kommentar || null,
        utfortAv: ferdigmeldForm.utfortAv || null,
        forbruk: ferdigmeldForm.forbruk.length > 0 ? ferdigmeldForm.forbruk.map(f => ({
          artikkelId: f.artikkelId, lotNr: f.lotNr, mengdeBrukt: f.mengdeBrukt,
          enhet: f.enhet || null, overstyrt: f.overstyrt, kommentar: f.kommentar || null,
        })) : null,
      });
      setShowFerdigmeldModal(false);
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
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
        <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}>+ Ny produksjonsordre</button>
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
                {o.status === 'IProduksjon' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => openFerdigmeld(o)}>Ferdigmeld</button>}
                {o.status !== 'Ferdigmeldt' && o.status !== 'Kansellert' && <button className="btn btn-sm btn-danger" onClick={() => setStatus(o.id, 'Kansellert')}>Kanseller</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Create modal */}
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

      {/* Ferdigmeld modal */}
      {showFerdigmeldModal && ferdigmeldOrdre && (
        <div className="modal-overlay" onClick={() => setShowFerdigmeldModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 700 }}>
            <h2>Ferdigmeld produksjon</h2>
            <form onSubmit={handleFerdigmeld}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Antall produsert *</label>
                  <input type="number" required min="0.01" step="0.01" value={ferdigmeldForm.antallProdusert}
                    onChange={e => setFerdigmeldForm({ ...ferdigmeldForm, antallProdusert: parseFloat(e.target.value) || 0 })} />
                </div>
                <div className="form-group">
                  <label>Utfort av</label>
                  <input value={ferdigmeldForm.utfortAv} onChange={e => setFerdigmeldForm({ ...ferdigmeldForm, utfortAv: e.target.value })} />
                </div>
              </div>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Kommentar</label>
                <textarea rows={2} value={ferdigmeldForm.kommentar} onChange={e => setFerdigmeldForm({ ...ferdigmeldForm, kommentar: e.target.value })} />
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowFerdigmeldModal(false)}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Ferdigmeld</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
