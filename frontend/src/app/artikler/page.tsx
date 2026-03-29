'use client';
import { useEffect, useState, Fragment } from 'react';
import { Article, CreateArticle, UpdateArticle, LagerBeholdning, get, post, put, del } from '../../lib/api';

const ARTICLE_TYPES = ['Råvare', 'Ferdigvare', 'Halvfabrikat', 'Emballasje', 'Annet'];
const ENHETER = ['STK', 'KG', 'L', 'M', 'SETT', 'PAKKE', 'BOKS'];

export default function ArtiklerPage() {
  const [articles, setArticles] = useState<Article[]>([]);
  const [beholdning, setBeholdning] = useState<LagerBeholdning[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [typeFilter, setTypeFilter] = useState('');
  const [visInactive, setVisInactive] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editArticle, setEditArticle] = useState<Article | null>(null);
  const [expanded, setExpanded] = useState<number | null>(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [form, setForm] = useState<CreateArticle>({
    artikkelNr: '', navn: '', enhet: 'STK', type: 'Råvare',
    beskrivelse: '', strekkode: '', kategori: '',
    innpris: 0, utpris: 0, minBeholdning: 0,
  });

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const [a, b] = await Promise.all([
        get<Article[]>('/articles'),
        get<LagerBeholdning[]>('/inventory'),
      ]);
      setArticles(a);
      setBeholdning(b);
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

  function getArticleBeholdning(artikkelId: number) {
    return beholdning.filter(b => b.artikkelId === artikkelId);
  }

  function getTotalBeholdning(artikkelId: number): number {
    return getArticleBeholdning(artikkelId).reduce((sum, b) => sum + b.mengde, 0);
  }

  function openCreate() {
    setEditArticle(null);
    setForm({ artikkelNr: '', navn: '', enhet: 'STK', type: 'Råvare', beskrivelse: '', strekkode: '', kategori: '', innpris: 0, utpris: 0, minBeholdning: 0 });
    setError('');
    setShowModal(true);
  }

  function openEdit(a: Article) {
    setEditArticle(a);
    setForm({
      artikkelNr: a.artikkelNr, navn: a.navn, enhet: a.enhet, type: a.type,
      beskrivelse: a.beskrivelse ?? '', strekkode: a.strekkode ?? '', kategori: a.kategori ?? '',
      innpris: a.innpris, utpris: a.utpris, minBeholdning: a.minBeholdning,
    });
    setError('');
    setShowModal(true);
  }

  function closeModal() { setShowModal(false); setEditArticle(null); setError(''); }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.artikkelNr.trim()) { setError('Artikkelnr er påkrevd.'); return; }
    if (!form.navn.trim()) { setError('Navn er påkrevd.'); return; }
    setError('');
    try {
      if (editArticle) {
        await put(`/articles/${editArticle.id}`, { ...form, aktiv: editArticle.aktiv } as UpdateArticle);
        showSuccess('Artikkel oppdatert.');
      } else {
        await post('/articles', form);
        showSuccess('Artikkel opprettet.');
      }
      closeModal(); load();
    } catch (err) { setError('Feil: ' + (err as Error).message); }
  }

  async function handleDelete(id: number) {
    if (!confirm('Er du sikker på at du vil slette denne artikkelen?')) return;
    try { await del(`/articles/${id}`); load(); }
    catch (err) { alert('Feil ved sletting: ' + (err as Error).message); }
  }

  async function handleToggleActive(a: Article) {
    const nyAktiv = !a.aktiv;
    try {
      await put(`/articles/${a.id}`, {
        artikkelNr: a.artikkelNr, navn: a.navn, enhet: a.enhet, type: a.type,
        beskrivelse: a.beskrivelse ?? '', strekkode: a.strekkode ?? '', kategori: a.kategori ?? '',
        innpris: a.innpris, utpris: a.utpris, minBeholdning: a.minBeholdning, aktiv: nyAktiv,
      } as UpdateArticle);
      load();
    } catch (err) { alert('Feil: ' + (err as Error).message); }
  }

  function showSuccess(msg: string) { setSuccess(msg); setTimeout(() => setSuccess(''), 3000); }

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
        <input placeholder="Søk artikkelnr, navn, kategori..." value={search} onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 260, fontSize: '0.9rem', minHeight: 40 }} />
        <select value={typeFilter} onChange={e => setTypeFilter(e.target.value)}
          style={{ padding: '0.35rem 0.6rem', border: '1px solid #d1d5db', borderRadius: 6, fontSize: '0.9rem' }}>
          <option value="">Alle typer</option>
          {uniqueTypes.map(t => <option key={t} value={t}>{t}</option>)}
        </select>
        <label style={{ display: 'flex', alignItems: 'center', gap: 5, fontSize: '0.85rem', cursor: 'pointer', userSelect: 'none', color: '#374151' }}>
          <input type="checkbox" checked={visInactive} onChange={e => setVisInactive(e.target.checked)} />
          Vis inaktive
        </label>
        <span style={{ marginLeft: 'auto', fontSize: '0.85rem', color: '#6b7280' }}>{filtered.length} av {articles.length} artikler</span>
      </div>
      {loading ? <div className="loading">Laster artikler...</div>
        : filtered.length === 0 ? <div className="empty">Ingen artikler funnet</div>
        : (
        <div className="table-wrapper">
        <table>
          <thead>
            <tr><th></th><th>Artikkelnr</th><th>Navn</th><th>Type</th><th>Enhet</th><th>Lager</th><th>Innpris</th><th>Utpris</th><th>Min</th><th></th></tr>
          </thead>
          <tbody>
            {filtered.map(a => {
              const artBeh = getArticleBeholdning(a.id);
              const total = getTotalBeholdning(a.id);
              const erLav = a.minBeholdning > 0 && total < a.minBeholdning;
              return (
                <Fragment key={a.id}>
                  <tr style={{ opacity: a.aktiv ? 1 : 0.5, cursor: 'pointer' }} onClick={() => setExpanded(expanded === a.id ? null : a.id)}>
                    <td style={{ width: 28, textAlign: 'center' }}>
                      <span style={{ fontSize: '0.8rem', color: '#9ca3af' }}>{artBeh.length > 0 ? (expanded === a.id ? '▾' : '▸') : ''}</span>
                    </td>
                    <td><code>{a.artikkelNr}</code></td>
                    <td>{a.navn}</td>
                    <td>{typeBadge(a.type)}</td>
                    <td>{a.enhet}</td>
                    <td>
                      <span style={erLav ? { color: '#dc2626', fontWeight: 600 } : undefined}>
                        {total > 0 ? `${total.toFixed(2)} ${a.enhet}` : '—'}
                      </span>
                      {erLav && ' ⚠️'}
                    </td>
                    <td>{a.innpris > 0 ? `${a.innpris.toFixed(2)} kr` : '—'}</td>
                    <td>{a.utpris > 0 ? `${a.utpris.toFixed(2)} kr` : '—'}</td>
                    <td>{a.minBeholdning > 0 ? a.minBeholdning : '—'}</td>
                    <td onClick={e => e.stopPropagation()}>
                      <button className="btn btn-sm btn-secondary" style={{ marginRight: 4 }} onClick={() => openEdit(a)}>Rediger</button>
                      <button className="btn btn-sm btn-secondary" style={{ marginRight: 4 }} onClick={() => handleToggleActive(a)}>{a.aktiv ? 'Deaktiver' : 'Aktiver'}</button>
                      <button className="btn btn-sm btn-danger" onClick={() => handleDelete(a.id)}>Slett</button>
                    </td>
                  </tr>
                  {expanded === a.id && artBeh.length > 0 && (
                    <tr style={{ background: '#f9fafb' }}>
                      <td colSpan={10} style={{ padding: '0.5rem 1rem' }}>
                        <div style={{ fontSize: '0.8rem', color: '#6b7280', marginBottom: '0.25rem' }}>Lagerbeholdning:</div>
                        <table style={{ width: 'auto', background: '#fff', border: '1px solid #e5e7eb', borderRadius: 6, fontSize: '0.85rem' }}>
                          <thead><tr style={{ background: '#f3f4f6' }}>
                            <th style={{ padding: '0.25rem 0.6rem' }}>LotNr</th>
                            <th style={{ padding: '0.25rem 0.6rem' }}>Mengde</th>
                            <th style={{ padding: '0.25rem 0.6rem' }}>Lokasjon</th>
                            <th style={{ padding: '0.25rem 0.6rem' }}>Best-før</th>
                            <th style={{ padding: '0.25rem 0.6rem' }}>Oppdatert</th>
                          </tr></thead>
                          <tbody>
                            {artBeh.map(b => (
                              <tr key={b.id}>
                                <td style={{ padding: '0.25rem 0.6rem' }}><code>{b.lotNr}</code></td>
                                <td style={{ padding: '0.25rem 0.6rem', fontWeight: 600 }}>{b.mengde.toFixed(2)} {b.enhet}</td>
                                <td style={{ padding: '0.25rem 0.6rem' }}>{b.lokasjon ?? '—'}</td>
                                <td style={{ padding: '0.25rem 0.6rem' }}>{b.bestForDato ? new Date(b.bestForDato).toLocaleDateString('no-NO') : '—'}</td>
                                <td style={{ padding: '0.25rem 0.6rem' }}>{new Date(b.sistOppdatert).toLocaleDateString('no-NO')}</td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </td>
                    </tr>
                  )}
                  {expanded === a.id && artBeh.length === 0 && (
                    <tr style={{ background: '#f9fafb' }}>
                      <td colSpan={10} style={{ padding: '0.5rem 1rem', fontSize: '0.85rem', color: '#9ca3af' }}>
                        Ingen lagerbeholdning for denne artikkelen.
                      </td>
                    </tr>
                  )}
                </Fragment>
              );
            })}
          </tbody>
        </table>
        </div>
      )}
      {showModal && (
        <div className="modal-overlay" onClick={e => { if (e.target === e.currentTarget) closeModal(); }}>
          <div className="modal">
            <h2>{editArticle ? 'Rediger artikkel' : 'Ny artikkel'}</h2>
            <form onSubmit={handleSubmit}>
              {error && <div className="alert alert-error">{error}</div>}
              <div className="form-grid">
                <div className="form-group"><label>Artikkelnr *</label><input value={form.artikkelNr} onChange={e => setForm({ ...form, artikkelNr: e.target.value })} placeholder="f.eks. ART-001" required /></div>
                <div className="form-group"><label>Navn *</label><input value={form.navn} onChange={e => setForm({ ...form, navn: e.target.value })} placeholder="Navn på artikkel" required /></div>
                <div className="form-group"><label>Type</label><select value={form.type} onChange={e => setForm({ ...form, type: e.target.value })}>{ARTICLE_TYPES.map(t => <option key={t} value={t}>{t}</option>)}</select></div>
                <div className="form-group"><label>Kategori</label><input value={form.kategori ?? ''} onChange={e => setForm({ ...form, kategori: e.target.value })} placeholder="f.eks. Meieri, Kjøtt" list="kategorier" /><datalist id="kategorier">{Array.from(new Set(articles.map(a => a.kategori).filter(Boolean))).sort().map(k => <option key={k!} value={k!} />)}</datalist></div>
                <div className="form-group"><label>Enhet</label><select value={form.enhet} onChange={e => setForm({ ...form, enhet: e.target.value })}>{ENHETER.map(eu => <option key={eu} value={eu}>{eu}</option>)}</select></div>
                <div className="form-group"><label>Strekkode</label><input value={form.strekkode ?? ''} onChange={e => setForm({ ...form, strekkode: e.target.value })} placeholder="Strekkode / EAN" /></div>
                <div className="form-group"><label>Innpris (kr)</label><input type="number" step="0.01" min="0" value={form.innpris} onChange={e => setForm({ ...form, innpris: parseFloat(e.target.value) || 0 })} /></div>
                <div className="form-group"><label>Utpris (kr)</label><input type="number" step="0.01" min="0" value={form.utpris} onChange={e => setForm({ ...form, utpris: parseFloat(e.target.value) || 0 })} /></div>
                <div className="form-group"><label>Min. beholdning</label><input type="number" min="0" value={form.minBeholdning} onChange={e => setForm({ ...form, minBeholdning: parseInt(e.target.value) || 0 })} /></div>
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
