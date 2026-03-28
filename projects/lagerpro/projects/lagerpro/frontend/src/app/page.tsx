'use client';
import { useEffect, useState } from 'react';
import { Article, CreateArticle, UpdateArticle, get, post, put, del } from '../lib/api';

const ARTICLE_TYPES = ['Råvare', 'Halvfabrikat', 'Ferdigvare', 'Emballasje', 'Hjelpemiddel'];

export default function ArticlesPage() {
  const [articles, setArticles] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [search, setSearch] = useState('');
  const [saving, setSaving] = useState(false);

  const [form, setForm] = useState<CreateArticle>({
    artikkelNr: '', navn: '', enhet: 'STK', type: 'Ferdigvare',
    beskrivelse: '', strekkode: '', kategori: '',
    innpris: 0, utpris: 0, minBeholdning: 0,
  });

  useEffect(() => { loadArticles(); }, []);

  async function loadArticles() {
    try {
      const data = await get<Article[]>('/articles');
      setArticles(data);
    } catch (e: unknown) {
      setError('Kunne ikke laste artikler: ' + (e instanceof Error ? e.message : String(e)));
    } finally {
      setLoading(false);
    }
  }

  function openCreate() {
    setEditId(null);
    setForm({ artikkelNr: '', navn: '', enhet: 'STK', type: 'Ferdigvare', beskrivelse: '', strekkode: '', kategori: '', innpris: 0, utpris: 0, minBeholdning: 0 });
    setShowModal(true);
  }

  function openEdit(a: Article) {
    setEditId(a.id);
    setForm({
      artikkelNr: a.artikkelNr, navn: a.navn, enhet: a.enhet, type: a.type,
      beskrivelse: a.beskrivelse ?? '', strekkode: a.strekkode ?? '', kategori: a.kategori ?? '',
      innpris: a.innpris, utpris: a.utpris, minBeholdning: a.minBeholdning,
    });
    setShowModal(true);
  }

  async function handleSave(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError('');
    try {
      if (editId) {
        await put(`/articles/${editId}`, { ...form, aktiv: articles.find(a => a.id === editId)?.aktiv ?? true } as UpdateArticle);
      } else {
        await post('/articles', form);
      }
      setShowModal(false);
      loadArticles();
    } catch (e: unknown) {
      setError('Lagring feilet: ' + (e instanceof Error ? e.message : String(e)));
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id: number) {
    if (!confirm('Slett artikkel?')) return;
    try {
      await del(`/articles/${id}`);
      loadArticles();
    } catch (e: unknown) {
      alert('Kunne ikke slette: ' + (e instanceof Error ? e.message : String(e)));
    }
  }

  const filtered = articles.filter(a =>
    a.navn.toLowerCase().includes(search.toLowerCase()) ||
    a.artikkelNr.toLowerCase().includes(search.toLowerCase()) ||
    (a.kategori ?? '').toLowerCase().includes(search.toLowerCase())
  );

  if (loading) return <div className="loading">Laster artikler...</div>;

  return (
    <>
      <div className="page-header">
        <h1>📦 Artikler</h1>
        <button className="btn btn-primary" onClick={openCreate}>+ Ny artikkel</button>
      </div>

      <div style={{ marginBottom: '1rem' }}>
        <input
          placeholder="Søk artikkelnavn, nummer eller kategori..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 320, fontSize: '0.9rem' }}
        />
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <table>
        <thead>
          <tr>
            <th>Artikkelnr</th>
            <th>Navn</th>
            <th>Type</th>
            <th>Enhet</th>
            <th>Kategori</th>
            <th>Innpris</th>
            <th>Utpris</th>
            <th>Min beh.</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={10} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen artikler funnet</td></tr>
          ) : filtered.map(a => (
            <tr key={a.id}>
              <td><code>{a.artikkelNr}</code></td>
              <td>{a.navn}</td>
              <td>{a.type}</td>
              <td>{a.enhet}</td>
              <td>{a.kategori ?? '—'}</td>
              <td>{a.innpris.toFixed(2)}</td>
              <td>{a.utpris.toFixed(2)}</td>
              <td>{a.minBeholdning}</td>
              <td><span className={`badge ${a.aktiv ? 'badge-aktiv' : 'badge-inactive'}`}>{a.aktiv ? 'Aktiv' : 'Inaktiv'}</span></td>
              <td>
                <button className="btn btn-sm btn-secondary" style={{ marginRight: 4 }} onClick={() => openEdit(a)}>Rediger</button>
                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(a.id)}>Slett</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>{editId ? 'Rediger artikkel' : 'Ny artikkel'}</h2>
            <form onSubmit={handleSave}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Artikkelnr *</label>
                  <input required value={form.artikkelNr} onChange={e => setForm({ ...form, artikkelNr: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Navn *</label>
                  <input required value={form.navn} onChange={e => setForm({ ...form, navn: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Type</label>
                  <select value={form.type} onChange={e => setForm({ ...form, type: e.target.value })}>
                    {ARTICLE_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Enhet</label>
                  <input value={form.enhet} onChange={e => setForm({ ...form, enhet: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Kategori</label>
                  <input value={form.kategori ?? ''} onChange={e => setForm({ ...form, kategori: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Strekkode</label>
                  <input value={form.strekkode ?? ''} onChange={e => setForm({ ...form, strekkode: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Innpris (kr)</label>
                  <input type="number" step="0.01" min="0" value={form.innpris} onChange={e => setForm({ ...form, innpris: parseFloat(e.target.value) || 0 })} />
                </div>
                <div className="form-group">
                  <label>Utpris (kr)</label>
                  <input type="number" step="0.01" min="0" value={form.utpris} onChange={e => setForm({ ...form, utpris: parseFloat(e.target.value) || 0 })} />
                </div>
                <div className="form-group">
                  <label>Min. beholdning</label>
                  <input type="number" min="0" value={form.minBeholdning} onChange={e => setForm({ ...form, minBeholdning: parseInt(e.target.value) || 0 })} />
                </div>
              </div>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Beskrivelse</label>
                <textarea rows={3} value={form.beskrivelse ?? ''} onChange={e => setForm({ ...form, beskrivelse: e.target.value })} />
              </div>
              {error && <div className="alert alert-error">{error}</div>}
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>Avbryt</button>
                <button type="submit" className="btn btn-primary" disabled={saving}>{saving ? 'Lagrer...' : 'Lagre'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
