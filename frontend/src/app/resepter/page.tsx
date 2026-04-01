'use client';
import { useEffect, useState } from 'react';
import { Article, Resept, get, post, put, del } from '../../lib/api';
import { Fragment } from 'react';

interface ReseptLinjeForm {
  ravareId: number;
  mengde: number;
  enhet: string;
  rekkefolge: number;
  kommentar: string;
}

interface ReseptForm {
  navn: string;
  ferdigvareId: number;
  beskrivelse: string;
  antallPortjoner: number;
  instruksjoner: string;
  linjer: ReseptLinjeForm[];
}

export default function ResepterPage() {
  const [resepter, setResepter] = useState<Resept[]>([]);
  const [articles, setArticles] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [visKunAktive, setVisKunAktive] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editResept, setEditResept] = useState<Resept | null>(null);
  const [expanded, setExpanded] = useState<number | null>(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const emptyForm: ReseptForm = {
    navn: '', ferdigvareId: 0, beskrivelse: '', antallPortjoner: 1,
    instruksjoner: '', linjer: [{ ravareId: 0, mengde: 0, enhet: 'KG', rekkefolge: 1, kommentar: '' }],
  };
  const [form, setForm] = useState<ReseptForm>(emptyForm);

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const [r, a] = await Promise.all([get<Resept[]>('/recipes'), get<Article[]>('/articles')]);
      setResepter(r);
      setArticles(a);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  const filtered = resepter.filter(r => {
    const q = search.toLowerCase();
    return (
      (r.navn.toLowerCase().includes(q) ||
       (r.ferdigvareNavn ?? '').toLowerCase().includes(q)) &&
      (!visKunAktive || r.aktiv)
    );
  });

  function openCreate() {
    setEditResept(null);
    setForm(emptyForm);
    setError('');
    setShowModal(true);
  }

  function openEdit(r: Resept) {
    setEditResept(r);
    setForm({
      navn: r.navn,
      ferdigvareId: r.ferdigvareId,
      beskrivelse: r.beskrivelse ?? '',
      antallPortjoner: r.antallPortjoner,
      instruksjoner: r.instruksjoner ?? '',
      linjer: r.linjer.length > 0
        ? r.linjer.map(l => ({ ravareId: l.ravareId, mengde: l.mengde, enhet: l.enhet, rekkefolge: l.rekkefolge, kommentar: l.kommentar ?? '' }))
        : [{ ravareId: 0, mengde: 0, enhet: 'KG', rekkefolge: 1, kommentar: '' }],
    });
    setError('');
    setShowModal(true);
  }

  function closeModal() {
    setShowModal(false);
    setEditResept(null);
    setError('');
  }

  function addLine() {
    setForm({ ...form, linjer: [...form.linjer, { ravareId: 0, mengde: 0, enhet: 'KG', rekkefolge: form.linjer.length + 1, kommentar: '' }] });
  }

  function removeLine(i: number) {
    setForm({ ...form, linjer: form.linjer.filter((_, idx) => idx !== i) });
  }

  function updateLine(i: number, field: string, value: number | string) {
    const linjer = form.linjer.map((l, idx) => idx === i ? { ...l, [field]: value } : l);
    setForm({ ...form, linjer });
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.navn.trim()) { setError('Navn er påkrevd.'); return; }
    if (!form.ferdigvareId) { setError('Velg ferdigvare.'); return; }
    if (form.linjer.some(l => !l.ravareId)) { setError('Velg råvare på alle linjer.'); return; }
    setError('');
    try {
      const payload = {
        navn: form.navn,
        ferdigvareId: form.ferdigvareId,
        beskrivelse: form.beskrivelse || undefined,
        antallPortjoner: form.antallPortjoner,
        instruksjoner: form.instruksjoner || undefined,
        linjer: form.linjer.map((l, idx) => ({
          ravareId: l.ravareId,
          mengde: l.mengde,
          enhet: l.enhet,
          rekkefolge: idx + 1,
          kommentar: l.kommentar || undefined,
        })),
      };
      if (editResept) {
        await put(`/recipes/${editResept.id}`, { ...payload, aktiv: editResept.aktiv });
        showSuccess('Resept oppdatert.');
      } else {
        await post('/recipes', payload);
        showSuccess('Resept opprettet.');
      }
      closeModal();
      load();
    } catch (err) { setError('Feil: ' + (err as Error).message); }
  }

  async function handleDelete(id: number) {
    if (!confirm('Er du sikker på at du vil slette denne resepten?')) return;
    try { await del(`/recipes/${id}`); load(); }
    catch (err) { alert('Feil: ' + (err as Error).message); }
  }

  async function handleToggleActive(r: Resept) {
    try {
      await put(`/recipes/${r.id}`, {
        navn: r.navn, ferdigvareId: r.ferdigvareId, beskrivelse: r.beskrivelse,
        antallPortjoner: r.antallPortjoner, instruksjoner: r.instruksjoner,
        aktiv: !r.aktiv, linjer: r.linjer.map(l => ({
          ravareId: l.ravareId, mengde: l.mengde, enhet: l.enhet,
          rekkefolge: l.rekkefolge, kommentar: l.kommentar,
        })),
      });
      load();
    } catch (err) { alert('Feil: ' + (err as Error).message); }
  }

  function showSuccess(msg: string) {
    setSuccess(msg);
    setTimeout(() => setSuccess(''), 3000);
  }

  const ferdigvarer = articles.filter(a => a.aktiv);

  if (loading) return <div className="loading">Laster resepter...</div>;

  return (
    <>
      <div className="page-header">
        <h1>📋 Resepter</h1>
        <button className="btn btn-primary" onClick={openCreate}>+ Ny resept</button>
      </div>

      {success && <div className="alert alert-success">{success}</div>}
      {error && <div className="alert alert-error">{error}</div>}

      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', marginBottom: '1rem', flexWrap: 'wrap' }}>
        <input
          placeholder="Søk reseptnavn..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 280, fontSize: '0.9rem' }}
        />
        <label style={{ display: 'flex', alignItems: 'center', gap: 6, fontSize: '0.9rem', cursor: 'pointer', userSelect: 'none' }}>
          <input type="checkbox" checked={visKunAktive} onChange={e => setVisKunAktive(e.target.checked)} />
          Vis kun aktive
        </label>
      </div>

      {filtered.length === 0 ? (
        <div style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen resepter funnet</div>
      ) : (
        <div className="table-wrapper">
        <table className="table-scroll">
          <thead>
            <tr><th>Navn</th><th>Ferdigvare</th><th>Porsjoner</th><th>Linjer</th><th>Status</th><th></th></tr>
          </thead>
          <tbody>
            {filtered.map(r => (
              <Fragment key={r.id}>
                <tr style={{ cursor: 'pointer' }} onClick={() => setExpanded(expanded === r.id ? null : r.id)}>
                  <td><strong>{r.navn}</strong></td>
                  <td>{r.ferdigvareNavn ?? `Art.ID ${r.ferdigvareId}`}</td>
                  <td>{r.antallPortjoner}</td>
                  <td>{r.linjer.length}</td>
                  <td><span className={`badge ${r.aktiv ? 'badge-aktiv' : 'badge-inactive'}`}>{r.aktiv ? 'Aktiv' : 'Inaktiv'}</span></td>
                  <td onClick={e => e.stopPropagation()}>
                    <div style={{ display: 'flex', gap: '0.4rem' }}>
                      <button className="btn btn-sm btn-secondary" onClick={() => openEdit(r)}>Rediger</button>
                      <button className="btn btn-sm btn-secondary" onClick={() => handleToggleActive(r)}>{r.aktiv ? 'Deaktiver' : 'Aktiver'}</button>
                      <button className="btn btn-sm btn-danger" onClick={() => handleDelete(r.id)}>Slett</button>
                    </div>
                  </td>
                </tr>
                {expanded === r.id && (
                  <tr style={{ background: '#f9fafb' }}>
                    <td colSpan={6} style={{ padding: '0.75rem 1rem' }}>
                      {r.beskrivelse && <p style={{ marginBottom: '0.5rem', color: '#6b7280', fontSize: '0.875rem' }}>{r.beskrivelse}</p>}
                      {r.linjer.length > 0 && (
                        <table className="table-scroll" style={{ width: 'auto', fontSize: '0.85rem', background: '#fff', border: '1px solid #e5e7eb', borderRadius: 6 }}>
                          <thead>
                            <tr style={{ background: '#f3f4f6' }}>
                              <th style={{ padding: '0.3rem 0.6rem' }}>#</th>
                              <th style={{ padding: '0.3rem 0.6rem' }}>Råvare</th>
                              <th style={{ padding: '0.3rem 0.6rem' }}>Mengde</th>
                              <th style={{ padding: '0.3rem 0.6rem' }}>Enhet</th>
                              <th style={{ padding: '0.3rem 0.6rem' }}>Kommentar</th>
                            </tr>
                          </thead>
                          <tbody>
                            {r.linjer.map(l => (
                              <tr key={l.id}>
                                <td style={{ padding: '0.3rem 0.6rem' }}>{l.rekkefolge}</td>
                                <td style={{ padding: '0.3rem 0.6rem' }}>{l.ravareNavn ?? `Råvare.ID ${l.ravareId}`}</td>
                                <td style={{ padding: '0.3rem 0.6rem' }}>{l.mengde}</td>
                                <td style={{ padding: '0.3rem 0.6rem' }}>{l.enhet}</td>
                                <td style={{ padding: '0.3rem 0.6rem', color: '#6b7280' }}>{l.kommentar ?? '—'}</td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      )}
                      {r.instruksjoner && (
                        <details style={{ marginTop: '0.5rem' }}>
                          <summary style={{ cursor: 'pointer', fontSize: '0.85rem', color: '#6b7280' }}>Instruksjoner</summary>
                          <pre style={{ marginTop: '0.5rem', whiteSpace: 'pre-wrap', fontSize: '0.85rem', color: '#374151' }}>{r.instruksjoner}</pre>
                        </details>
                      )}
                    </td>
                  </tr>
                )}
              </Fragment>
            ))}
          </tbody>
        </table>
        </div>
      )}

      {showModal && (
        <div className="modal-overlay" onClick={e => { if (e.target === e.currentTarget) closeModal(); }}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 700 }}>
            <h2>{editResept ? 'Rediger resept' : 'Ny resept'}</h2>
            <form onSubmit={handleSubmit}>
              {error && <div className="alert alert-error">{error}</div>}

              <div className="form-grid">
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Navn *</label>
                  <input value={form.navn} onChange={e => setForm({ ...form, navn: e.target.value })} placeholder="Navn på resepten" required />
                </div>
                <div className="form-group">
                  <label>Ferdigvare *</label>
                  <select value={form.ferdigvareId} onChange={e => setForm({ ...form, ferdigvareId: parseInt(e.target.value) })}>
                    <option value={0}>Velg ferdigvare...</option>
                    {ferdigvarer.map(a => <option key={a.id} value={a.id}>{a.navn} ({a.artikkelNr})</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Antall porsjoner</label>
                  <input type="number" min={1} value={form.antallPortjoner} onChange={e => setForm({ ...form, antallPortjoner: parseInt(e.target.value) || 1 })} />
                </div>
              </div>

              <div className="form-group" style={{ marginBottom: '0.75rem' }}>
                <label>Beskrivelse</label>
                <textarea rows={2} value={form.beskrivelse} onChange={e => setForm({ ...form, beskrivelse: e.target.value })} placeholder="Kort beskrivelse" />
              </div>

              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Instruksjoner</label>
                <textarea rows={3} value={form.instruksjoner} onChange={e => setForm({ ...form, instruksjoner: e.target.value })} placeholder="Produksjonsinstrukser..." />
              </div>

              <hr style={{ margin: '1rem 0' }} />
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                <strong>Råvarer</strong>
                <button type="button" className="btn btn-sm btn-secondary" onClick={addLine}>+ Linje</button>
              </div>

              {form.linjer.map((linje, i) => (
                <div key={i} style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr auto', gap: '0.5rem', marginBottom: '0.5rem', alignItems: 'end' }}>
                  <div className="form-group">
                    <label>Råvare</label>
                    <select value={linje.ravareId} onChange={e => {
                      const a = articles.find(x => x.id === parseInt(e.target.value));
                      updateLine(i, 'ravareId', parseInt(e.target.value));
                      if (a) updateLine(i, 'enhet', a.enhet);
                    }}>
                      <option value={0}>Velg...</option>
                      {articles.map(a => <option key={a.id} value={a.id}>{a.navn} ({a.artikkelNr})</option>)}
                    </select>
                  </div>
                  <div className="form-group">
                    <label>Mengde</label>
                    <input type="number" step="0.001" min="0" value={linje.mengde} onChange={e => updateLine(i, 'mengde', parseFloat(e.target.value))} />
                  </div>
                  <div className="form-group">
                    <label>Enhet</label>
                    <input value={linje.enhet} onChange={e => updateLine(i, 'enhet', e.target.value)} />
                  </div>
                  <div className="form-group">
                    <label>Kommentar</label>
                    <input value={linje.kommentar} onChange={e => updateLine(i, 'kommentar', e.target.value)} />
                  </div>
                  <button type="button" className="btn btn-sm btn-danger" onClick={() => removeLine(i)}>✕</button>
                </div>
              ))}

              <div className="form-actions" style={{ marginTop: '1rem' }}>
                <button type="button" className="btn btn-secondary" onClick={closeModal}>Avbryt</button>
                <button type="submit" className="btn btn-primary">{editResept ? 'Lagre' : 'Opprett'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
