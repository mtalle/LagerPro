'use client';
import { useEffect, useState } from 'react';
import { Leverandor, Kunde, get, post, put, del } from '../../lib/api';

export default function LeverandorerPage() {
  const [leverandorer, setLeverandorer] = useState<Leverandor[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [visKunAktive, setVisKunAktive] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editLev, setEditLev] = useState<Leverandor | null>(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [form, setForm] = useState({
    navn: '', kontaktperson: '', telefon: '', epost: '',
    adresse: '', postnr: '', poststed: '', orgNr: '', kommentar: '',
  });

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const data = await get<Leverandor[]>('/leverandorer');
      setLeverandorer(data);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  const filtered = leverandorer.filter(l => {
    const q = search.toLowerCase();
    return (
      (l.navn.toLowerCase().includes(q) ||
       (l.orgNr ?? '').toLowerCase().includes(q) ||
       (l.epost ?? '').toLowerCase().includes(q)) &&
      (!visKunAktive || l.aktiv)
    );
  });

  function openCreate() {
    setEditLev(null);
    setForm({ navn: '', kontaktperson: '', telefon: '', epost: '', adresse: '', postnr: '', poststed: '', orgNr: '', kommentar: '' });
    setError('');
    setShowModal(true);
  }

  function openEdit(l: Leverandor) {
    setEditLev(l);
    setForm({
      navn: l.navn,
      kontaktperson: l.kontaktperson ?? '',
      telefon: l.telefon ?? '',
      epost: l.epost ?? '',
      adresse: l.adresse ?? '',
      postnr: l.postnr ?? '',
      poststed: l.poststed ?? '',
      orgNr: l.orgNr ?? '',
      kommentar: l.kommentar ?? '',
    });
    setError('');
    setShowModal(true);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.navn.trim()) { setError('Navn er påkrevd.'); return; }
    setError('');
    try {
      if (editLev) {
        await put(`/leverandorer/${editLev.id}`, form);
        showSuccess('Leverandør oppdatert.');
      } else {
        await post('/leverandorer', form);
        showSuccess('Leverandør opprettet.');
      }
      setShowModal(false);
      load();
    } catch (err) { setError('Feil: ' + (err as Error).message); }
  }

  async function handleDelete(id: number) {
    if (!confirm('Er du sikker på at du vil slette denne leverandøren?')) return;
    try { await del(`/leverandorer/${id}`); load(); }
    catch (err) { alert('Feil: ' + (err as Error).message); }
  }

  async function handleToggleActive(l: Leverandor) {
    const updated = {
      navn: l.navn,
      kontaktperson: l.kontaktperson ?? '',
      telefon: l.telefon ?? '',
      epost: l.epost ?? '',
      adresse: l.adresse ?? '',
      postnr: l.postnr ?? '',
      poststed: l.poststed ?? '',
      orgNr: l.orgNr ?? '',
      kommentar: l.kommentar ?? '',
      aktiv: !l.aktiv,
    };
    try { await put(`/leverandorer/${l.id}`, updated); load(); }
    catch (err) { alert('Feil: ' + (err as Error).message); }
  }

  function showSuccess(msg: string) {
    setSuccess(msg);
    setTimeout(() => setSuccess(''), 3000);
  }

  return (
    <>
      <div className="page-header">
        <h1>🏭 Leverandører</h1>
        <button className="btn btn-primary" onClick={openCreate}>+ Ny leverandør</button>
      </div>

      {success && <div className="alert alert-success">{success}</div>}
      {error && <div className="alert alert-error">{error}</div>}

      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', marginBottom: '1rem', flexWrap: 'wrap' }}>
        <input
          placeholder="Søk navn, orgnr eller e-post..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 300, fontSize: '0.9rem' }}
        />
        <label style={{ display: 'flex', alignItems: 'center', gap: 6, fontSize: '0.9rem', cursor: 'pointer', userSelect: 'none' }}>
          <input type="checkbox" checked={visKunAktive} onChange={e => setVisKunAktive(e.target.checked)} />
          Vis kun aktive
        </label>
      </div>

      {loading ? (
        <div className="loading">Laster leverandører...</div>
      ) : filtered.length === 0 ? (
        <div className="empty">Ingen leverandører funnet</div>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Navn</th>
              <th>Org.nr</th>
              <th>Kontakt</th>
              <th>Telefon</th>
              <th>E-post</th>
              <th>Poststed</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {filtered.map(l => (
              <tr key={l.id}>
                <td><strong>{l.navn}</strong></td>
                <td><code>{l.orgNr ?? '—'}</code></td>
                <td>{l.kontaktperson ?? '—'}</td>
                <td>{l.telefon ?? '—'}</td>
                <td>{l.epost ?? '—'}</td>
                <td>{l.poststed ? `${l.postnr ?? ''} ${l.poststed}` : '—'}</td>
                <td>
                  <span className={`badge ${l.aktiv ? 'badge-aktiv' : 'badge-inactive'}`}>
                    {l.aktiv ? 'Aktiv' : 'Inaktiv'}
                  </span>
                </td>
                <td>
                  <div style={{ display: 'flex', gap: '0.4rem' }}>
                    <button className="btn btn-sm btn-secondary" onClick={() => openEdit(l)}>Rediger</button>
                    <button className="btn btn-sm btn-secondary" onClick={() => handleToggleActive(l)}>
                      {l.aktiv ? 'Deaktiver' : 'Aktiver'}
                    </button>
                    <button className="btn btn-sm btn-danger" onClick={() => handleDelete(l.id)}>Slett</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showModal && (
        <div className="modal-overlay" onClick={e => { if (e.target === e.currentTarget) setShowModal(false); }}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>{editLev ? 'Rediger leverandør' : 'Ny leverandør'}</h2>
            <form onSubmit={handleSubmit}>
              {error && <div className="alert alert-error">{error}</div>}
              <div className="form-grid">
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Navn *</label>
                  <input value={form.navn} onChange={e => setForm({ ...form, navn: e.target.value })} placeholder="Firmanavn" required />
                </div>
                <div className="form-group">
                  <label>Kontaktperson</label>
                  <input value={form.kontaktperson} onChange={e => setForm({ ...form, kontaktperson: e.target.value })} placeholder="Navn på kontaktperson" />
                </div>
                <div className="form-group">
                  <label>Telefon</label>
                  <input value={form.telefon} onChange={e => setForm({ ...form, telefon: e.target.value })} placeholder="+47 ..." />
                </div>
                <div className="form-group">
                  <label>E-post</label>
                  <input type="email" value={form.epost} onChange={e => setForm({ ...form, epost: e.target.value })} placeholder="epost@firma.no" />
                </div>
                <div className="form-group">
                  <label>Organisasjonsnr</label>
                  <input value={form.orgNr} onChange={e => setForm({ ...form, orgNr: e.target.value })} placeholder="999 999 999" />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Adresse</label>
                  <input value={form.adresse} onChange={e => setForm({ ...form, adresse: e.target.value })} placeholder="Gateadresse" />
                </div>
                <div className="form-group">
                  <label>Postnr</label>
                  <input value={form.postnr} onChange={e => setForm({ ...form, postnr: e.target.value })} placeholder="1234" />
                </div>
                <div className="form-group">
                  <label>Poststed</label>
                  <input value={form.poststed} onChange={e => setForm({ ...form, poststed: e.target.value })} placeholder="Oslo" />
                </div>
                <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                  <label>Kommentar</label>
                  <textarea rows={2} value={form.kommentar} onChange={e => setForm({ ...form, kommentar: e.target.value })} placeholder="Valgfri kommentar" />
                </div>
              </div>
              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>Avbryt</button>
                <button type="submit" className="btn btn-primary">{editLev ? 'Lagre' : 'Opprett'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
