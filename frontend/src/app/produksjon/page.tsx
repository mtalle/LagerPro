'use client';
import { useEffect, useState } from 'react';
import { ProduksjonsOrdre, Resept, ReseptLinje, FerdigmeldRequest, get, post, patch } from '../../lib/api';

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
  const [ferdigmeldOrdre, setFerdigmeldOrdre] = useState<ProduksjonsOrdre | null>(null);
  const [ferdigmeldResept, setFerdigmeldResept] = useState<Resept | null>(null);

  const [form, setForm] = useState({
    reseptId: 0, ordreNr: '', planlagtDato: new Date().toISOString().slice(0, 10),
    kommentar: '',
  });

  // Ferdigmeld form
  const [ferdigAntall, setFerdigAntall] = useState(0);
  const [ferdigUtfort, setFerdigUtfort] = useState('');
  const [ferdigKommentar, setFerdigKommentar] = useState('');

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

  function apneFerdigmeld(o: ProduksjonsOrdre) {
    const resept = resepter.find(r => r.id === o.reseptId);
    setFerdigmeldOrdre(o);
    setFerdigmeldResept(resept ?? null);
    setFerdigAntall(resept?.antallPortjoner ?? o.antallProdusert);
    setFerdigUtfort('');
    setFerdigKommentar('');
  }

  function lukkFerdigmeld() {
    setFerdigmeldOrdre(null);
    setFerdigmeldResept(null);
  }

  async function submitFerdigmeld(e: React.FormEvent) {
    e.preventDefault();
    if (!ferdigmeldOrdre) return;
    const req: FerdigmeldRequest = {
      antallProdusert: ferdigAntall,
      kommentar: ferdigKommentar || undefined,
      utfortAv: ferdigUtfort || undefined,
    };
    try {
      await post(`/production/${ferdigmeldOrdre.id}/ferdigmeld`, req);
      lukkFerdigmeld();
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  function calcLinjeMengde(linje: ReseptLinje): number {
    if (!ferdigmeldResept || ferdigmeldResept.antallPortjoner === 0) return linje.mengde;
    return (linje.mengde * ferdigAntall) / ferdigmeldResept.antallPortjoner;
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
                {o.status === 'IProduksjon' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => apneFerdigmeld(o)}>Ferdigmeld</button>}
                {o.status !== 'Ferdigmeldt' && o.status !== 'Kansellert' && <button className="btn btn-sm btn-danger" onClick={() => setStatus(o.id, 'Kansellert')}>Kanseller</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* New order modal */}
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

      {/* Ferdigmeld modal */}
      {ferdigmeldOrdre && (
        <div className="modal-overlay" onClick={lukkFerdigmeld}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 520 }}>
            <h2>✅ Ferdigmeld produksjon</h2>
            <form onSubmit={submitFerdigmeld}>
              <div style={{ background: '#f3f4f6', borderRadius: 8, padding: '0.75rem 1rem', marginBottom: '1rem' }}>
                <div style={{ fontWeight: 600, marginBottom: 4 }}>{ferdigmeldOrdre.reseptNavn ?? `Resept.ID ${ferdigmeldOrdre.reseptId}`}</div>
                {ferdigmeldResept && <div style={{ color: '#6b7280', fontSize: '0.875rem' }}>Standard: {ferdigmeldResept.antallPortjoner} porsjoner</div>}
              </div>

              <div className="form-grid">
                <div className="form-group">
                  <label>Antall produsert *</label>
                  <input
                    type="number"
                    min={1}
                    required
                    value={ferdigAntall}
                    onChange={e => setFerdigAntall(parseInt(e.target.value) || 0)}
                  />
                </div>
                <div className="form-group">
                  <label>Utfort av</label>
                  <input
                    type="text"
                    value={ferdigUtfort}
                    onChange={e => setFerdigUtfort(e.target.value)}
                    placeholder="Navn på produsent"
                  />
                </div>
              </div>

              <div className="form-group">
                <label>Kommentar</label>
                <textarea
                  rows={2}
                  value={ferdigKommentar}
                  onChange={e => setFerdigKommentar(e.target.value)}
                  placeholder="Eventuelle kommentarer eller avvik..."
                />
              </div>

              {ferdigmeldResept && ferdigmeldResept.linjer.length > 0 && (
                <div className="form-group">
                  <label>Forventet forbruk (readonly)</label>
                  <table style={{ width: '100%', fontSize: '0.875rem', borderCollapse: 'collapse', background: '#fff', border: '1px solid #e5e7eb', borderRadius: 6 }}>
                    <thead>
                      <tr style={{ background: '#f9fafb' }}>
                        <th style={{ padding: '0.4rem 0.6rem', textAlign: 'left' }}>Råvare</th>
                        <th style={{ padding: '0.4rem 0.6rem', textAlign: 'right' }}>Mengde</th>
                        <th style={{ padding: '0.4rem 0.6rem', textAlign: 'left' }}>Enhet</th>
                      </tr>
                    </thead>
                    <tbody>
                      {ferdigmeldResept.linjer.map(linje => (
                        <tr key={linje.id}>
                          <td style={{ padding: '0.4rem 0.6rem' }}>{linje.ravareNavn ?? `Råvare.ID ${linje.ravareId}`}</td>
                          <td style={{ padding: '0.4rem 0.6rem', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                            {calcLinjeMengde(linje).toFixed(2)}
                          </td>
                          <td style={{ padding: '0.4rem 0.6rem' }}>{linje.enhet}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}

              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={lukkFerdigmeld}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Ferdigmeld</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
