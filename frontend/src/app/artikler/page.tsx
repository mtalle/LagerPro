'use client';
import { useEffect, useState } from 'react';
import { Article, CreateArticle, UpdateArticle, get, post, put, patch, del } from '../../lib/api';

const ARTICLE_TYPES = ['Råvare', 'Ferdigvare', 'Halvfabrikat', 'Emballasje', 'Annet'];
const ENHETER = ['STK', 'KG', 'L', 'M', 'SETT', 'PAKKE', 'BOKS'];

export default function ArtiklerPage() {
  const [articles, setArticles] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [visKunAktive, setVisKunAktive] = useState(true);
  const [typeFilter, setTypeFilter] = useState('');
  const [visInactive, setVisInactive] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editArticle, setEditArticle] = useState<Article | null>(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Form state
  const [form, setForm] = useState<CreateArticle>({
    artikkelNr: '', navn: '', enhet: 'STK', type: 'Råvare',
    beskrivelse: '', strekkode: '', kategori: '',
    innpris: 0, utpris: 0, minBeholdning: 0,
  });

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const data = await get<Article[]>('/articles');
      setArticles(data);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  const filtered = articles.filter(a => {
    const q = search.toLowerCase();
    const matchesSearch =
      a.artikkelNr.toLowerCase().includes(q) ||
      a.navn.toLowerCase().includes(q) ||
      (a.kategori ?? '').toLowerCase().includes(q) ||
      (a.strekkode ?? '').toLowerCase().includes(q);
    const matchesType = !typeFilter || a.type === typeFilter;
    const matchesActive = visInactive ? true : a.aktiv;
    return matchesSearch && matchesType && matchesActive;
  });

  const uniqueTypes = Array.from(new Set(articles.map(a => a.type))).sort();

  function openCreate() {
    setEditArticle(null);
    setForm({ artikkelNr: '', navn: '', enhet: 'STK', type: 'Råvare', beskrivelse: '', strekkode: '', kategori: '', innpris: 0, utpris: 0, minBeholdning: 0 });
    setError('');
    setShowModal(true);
  }

  function openEdit(a: Article) {
    setEditArticle(a);
    setForm({
      artikkelNr: a.artikkelNr,
      navn: a.navn,
      enhet: a.enhet,
      type: a.type,
      beskrivelse: a.beskrivelse ?? '',
      strekkode: a.strekkode ?? '',
      kategori: a.kategori ?? '',
      innpris: a.innpris,
      utpris: a.utpris,
      minBeholdning: a.minBeholdning,
    });
    setError('');
    setShowModal(true);
  }

  function closeModal() {
    setShowModal(false);
    setEditArticle(null);
    setError('');
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.artikkelNr.trim()) { setError('Artikkelnr er påkrevd.'); return; }
    if (!form.navn.trim()) { setError('Navn er påkrevd.'); return; }
    setError('');
    try {
      if (editArticle) {
        const update: UpdateArticle = { ...form, aktiv: editArticle.aktiv };
        await put(`/articles/${editArticle.id}`, update);
        showSuccess('Artikkel oppdatert.');
      } else {
        await post('/articles', form);
        showSuccess('Artikkel opprettet.');
      }
      closeModal();
      load();
    } catch (err) { setError('Feil: ' + (err as Error).message); }
  }

  async function handleDelete(id: number) {
    if (!confirm('Er du sikker på at du vil slette denne artikkelen?')) return;
    try { await del(`/articles/${id}`); load(); }
    catch (err) { alert('Feil ved sletting: ' + (err as Error).message); }
  }

  async function handleToggleActive(a: Article) {
    const nyAktiv = !a.aktiv;
    const update: UpdateArticle = {
      artikkelNr: a.artikkelNr, navn: a.navn, enhet: a.enhet, type: a.type,
      beskrivelse: a.beskrivelse ?? '', strekkode: a.strekkode ?? '', kategori: a.kategori ?? '',
      innpris: a.innpris, utpris: a.utpris, minBeholdning: a.minBeholdning, aktiv: nyAktiv,
    };
    try { await put(`/articles/${a.id}`, update); load(); }
    catch (err) { alert('Feil: ' + (err as Error).message); }
  }

  function showSuccess(msg: string) {
    setSuccess(msg);
    setTimeout(() => setSuccess(''), 3000);
  }

  const typeBadge = (t: string) => {
    const colors: Record<string, string> = {
      'Råvare': '#dbeafe', 'Ferdigvare': '#dcfce7', 'Halvfabrikat': '#fef9c3',
      'Emballasje': '#fce7f3', 'Annet': '#f3f4f6',
    };
    return <span style={{ background: colors[t] ?? '#f3f4f6', padding: '2px 8px', borderRadius: 12, fontSize: '0.75rem', color: '#374151' }}>{t}</span>;
  };

  return (
    <>
      <div className="page-header">
        <h1>📦 Artikler</h1>
        <button className="btn btn-primary" onClick={openCreate}>+ Ny artikkel</button>
      </div>

      {success && <div className="alert alert-success">{success}</div>}
      {error && <div className="alert alert-error">{error}</div>}

      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', marginBottom: '1rem', flexWrap: 'wrap' }}>
        <input
          placeholder="Søk artikkelnr, navn, kategori..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 260, fontSize: '0.9rem' }}
        />
        <select value={typeFilter} onChange={e => setTypeFilter(e.target.value)} style={{ padding: '0.35rem 0.6rem', border: '1px solid #d1d5db', borderRadius: 6, fontSize: '0.9rem' }}>
          <option value="">Alle typer</option>
          {uniqueTypes.map(t => <option key={t} value={t}>{t}</option>)}
        </select>
        <label style={{ display: 'flex', alignItems: 'center', gap: 5, fontSize: '0.85rem', cursor: 'pointer', userSelect: 'none', color: '#374151' }}>
          <input type="checkbox" checked={!visKunAktive} onChange={e => setVisKunAktive(!e.target.checked)} />
          Aktive
        </label>
        <label style={{ display: 'flex', alignItems: 'center', gap: 5, fontSize: '0.85rem', cursor: 'pointer', userSelect: 'none', color: '#374151' }}>
          <input type="checkbox" checked={visInactive} onChange={e => setVisInactive(e.target.checked)} />
          Vis inaktive
        </label>
        <span style={{ marginLeft: 'auto', fontSize: '0.85rem', color: '#6b7280' }}>{filtered.length} av {articles.length} artikler</span>
      </div>

      {loading ? (
        <div className="loading">Laster artikler...</div>
      ) : filtered.length === 0 ? (
        <div className="empty">Ingen artikler funnet</div>
      ) : (
        <table>
          <thead>
            <tr><th>Artikkelnr</th><th>Navn</th><th>Type</th><th>Enhet</th><th>Innpris</th><th>Utpris</th><th>Min.beh.</th><th>Status</th><th></th></tr>
          </thead>
          <tbody>
            {filtered.map(a => (
              <tr key={a.id} style={{ opacity: a.aktiv ? 1 : 0.5 }}>
                <td><code>{a.artikkelNr}</code></td>
                <td>{a.navn}</td>
                <td>{typeBadge(a.type)}</td>
                <td>{a.enhet}</td>
                <td>{a.innpris > 0 ? `${a.innpris.toFixed(2)} kr` : '—'}</td>
                <td>{a.utpris > 0 ? `${a.utpris.toFixed(2)} kr` : '—'}</td>
                <td>{a.minBeholdning > 0 ? a.minBeholdning : '—'}</td>
                <td>
                  <span className={`badge ${a.aktiv ? 'badge-aktiv' : 'badge-inactive'}`}>
                    {a.aktiv ? 'Aktiv' : 'Inaktiv'}
                  </span>
                </td>
                <td>
                  <button className="btn btn-sm btn-secondary" style={{ marginRight: 4 }} onClick={() => openEdit(a)}>Rediger</button>
                  <button className="btn btn-sm btn-secondary" style={{ marginRight: 4 }} onClick={() => handleToggleActive(a)}>
                    {a.aktiv ? 'Deaktiver' : 'Aktiver'}
                  </button>
                  <button className="btn btn-sm btn-danger" onClick={() => handleDelete(a.id)}>Slett</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showModal && (
        <div className="modal-overlay" onClick={e => { if (e.target === e.currentTarget) closeModal(); }}>
          <div className="modal">
            <h2>{editArticle ? 'Rediger artikkel' : 'Ny artikkel'}</h2>
            <form onSubmit={handleSubmit}>
              {error && <div className="alert alert-error">{error}</div>}

              <div className="form-grid">
                <div className="form-group">
                  <label>Artikkelnr *</label>
                  <input value={form.artikkelNr} onChange={e => setForm({ ...form, artikkelNr: e.target.value })} placeholder="f.eks. ART-001" required />
                </div>
                <div className="form-group">
                  <label>Navn *</label>
                  <input value={form.navn} onChange={e => setForm({ ...form, navn: e.target.value })} placeholder="Navn på artikkel" required />
                </div>
                <div className="form-group">
                  <label>Type</label>
                  <select value={form.type} onChange={e => setForm({ ...form, type: e.target.value })}>
                    {ARTICLE_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Kategori</label>
                  <input value={form.kategori ?? ''} onChange={e => setForm({ ...form, kategori: e.target.value })} placeholder="f.eks. Meieri, Kjøtt, Tømmer" list="kategorier" />
                  <datalist id="kategorier">
                    {Array.from(new Set(articles.map(a => a.kategori).filter(Boolean))).sort().map(k => <option key={k!} value={k!} />)}
                  </datalist>
                </div>
                <div className="form-group">
                  <label>Enhet</label>
                  <select value={form.enhet} onChange={e => setForm({ ...form, enhet: e.target.value })}>
                    {ENHETER.map(e => <option key={e} value={e}>{e}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Strekkode</label>
                  <input value={form.strekkode ?? ''} onChange={e => setForm({ ...form, strekkode: e.target.value })} placeholder="Strekkode / EAN" />
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
                <textarea rows={3} value={form.beskrivelse ?? ''} onChange={e => setForm({ ...form, beskrivelse: e.target.value })} placeholder="Valgfri beskrivelse" />
              </div>

              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={closeModal}>Avbryt</button>
                <button type="submit" className="btn btn-primary">{editArticle ? 'Lagre' : 'Opprett'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
