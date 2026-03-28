'use client';
import { useEffect, useState, Fragment } from 'react';
import { Mottak, Article, Leverandor, get, post, patch } from '../../lib/api';

export default function MottakPage() {
  const [mottak, setMottak] = useState<Mottak[]>([]);
  const [articles, setArticles] = useState<Article[]>([]);
  const [leverandorer, setLeverandorer] = useState<Leverandor[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [expanded, setExpanded] = useState<number | null>(null);
  const [error, setError] = useState('');

  const [form, setForm] = useState({
    leverandorId: 0, mottaksDato: new Date().toISOString().slice(0, 10),
    referanse: '', kommentar: '', mottattAv: '',
    linjer: [{ artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK', bestForDato: '', temperatur: 0, strekkode: '', avvik: '', kommentar: '' }],
  });

  useEffect(() => { loadData(); }, []);

  async function loadData() {
    try {
      const [m, a, l] = await Promise.all([
        get<Mottak[]>('/mottak'),
        get<Article[]>('/articles'),
        get<Leverandor[]>('/leverandorer'),
      ]);
      setMottak(m);
      setArticles(a);
      setLeverandorer(l);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  function addLine() {
    setForm({ ...form, linjer: [...form.linjer, { artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK', bestForDato: '', temperatur: 0, strekkode: '', avvik: '', kommentar: '' }] });
  }

  function updateLine(i: number, field: string, value: unknown) {
    const lines = [...form.linjer];
    (lines[i] as Record<string, unknown>)[field] = value;
    setForm({ ...form, linjer: lines });
  }

  function removeLine(i: number) {
    setForm({ ...form, linjer: form.linjer.filter((_, idx) => idx !== i) });
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.leverandorId) { setError('Velg en leverandør.'); return; }
    if (form.linjer.some(l => !l.artikkelId)) { setError('Velg artikkel på alle linjer.'); return; }
    setError('');
    try {
      await post('/mottak', {
        leverandorId: form.leverandorId,
        mottaksDato: new Date(form.mottaksDato).toISOString(),
        referanse: form.referanse,
        kommentar: form.kommentar,
        mottattAv: form.mottattAv,
        linjer: form.linjer.map(l => ({
          artikkelId: l.artikkelId,
          lotNr: l.lotNr,
          mengde: l.mengde,
          enhet: l.enhet,
          bestForDato: l.bestForDato ? new Date(l.bestForDato).toISOString() : null,
          temperatur: l.temperatur,
          strekkode: l.strekkode,
          avvik: l.avvik,
          kommentar: l.kommentar,
        })),
      });
      setShowModal(false);
      loadData();
    } catch (e) { setError('Feil: ' + (e as Error).message); }
  }

  async function updateStatus(id: number, status: string) {
    try {
      await patch(`/mottak/${id}/status`, { status });
      loadData();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  const statusBadge = (s: string) => {
    const map: Record<string, string> = {
      Registrert: 'badge-registrert', Mottatt: 'badge-aktiv', Godkjent: 'badge-aktiv',
      'Delvis mottatt': 'badge-planlagt', Kansellert: 'badge-kansellert',
    };
    return <span className={`badge ${map[s] ?? ''}`}>{s}</span>;
  };

  if (loading) return <div className="loading">Laster mottak...</div>;

  return (
    <>
      <div className="page-header">
        <h1>📥 Mottak</h1>
        <button className="btn btn-primary" onClick={() => { setForm({ leverandorId: 0, mottaksDato: new Date().toISOString().slice(0, 10), referanse: '', kommentar: '', mottattAv: '', linjer: [{ artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK', bestForDato: '', temperatur: 0, strekkode: '', avvik: '', kommentar: '' }] }); setShowModal(true); }}>+ Nytt mottak</button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <table>
        <thead>
          <tr><th>ID</th><th>Dato</th><th>Leverandør</th><th>Referanse</th><th>Status</th><th>Mottatt av</th><th></th></tr>
        </thead>
        <tbody>
          {mottak.length === 0 ? (
            <tr><td colSpan={7} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen mottak</td></tr>
          ) : mottak.map(m => (
            <tr key={m.id} style={{ cursor: 'pointer' }} onClick={() => setExpanded(expanded === m.id ? null : m.id)}>
              <td>#{m.id}</td>
              <td>{new Date(m.mottaksDato).toLocaleDateString('no-NO')}</td>
              <td>{m.leverandorNavn ?? `Lev.ID ${m.leverandorId}`}</td>
              <td>{m.referanse ?? '—'}</td>
              <td>{statusBadge(m.status)}</td>
              <td>{m.mottattAv ?? '—'}</td>
              <td>
                {m.status === 'Registrert' && <button className="btn btn-sm btn-primary" onClick={e => { e.stopPropagation(); updateStatus(m.id, 'Mottatt'); }}>Mottatt</button>}
                {m.status === 'Mottatt' && <button className="btn btn-sm btn-primary" onClick={e => { e.stopPropagation(); updateStatus(m.id, 'Godkjent'); }}>Godkjenn</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 700 }}>
            <h2>Nytt mottak</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Leverandør *</label>
                  <select required value={form.leverandorId} onChange={e => setForm({ ...form, leverandorId: parseInt(e.target.value) })}>
                    <option value={0}>Velg leverandør...</option>
                    {leverandorer.map(lv => <option key={lv.id} value={lv.id}>{lv.navn}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Mottaksdato</label>
                  <input type="date" value={form.mottaksDato} onChange={e => setForm({ ...form, mottaksDato: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Referanse</label>
                  <input value={form.referanse} onChange={e => setForm({ ...form, referanse: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Mottatt av</label>
                  <input value={form.mottattAv} onChange={e => setForm({ ...form, mottattAv: e.target.value })} />
                </div>
              </div>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Kommentar</label>
                <textarea rows={2} value={form.kommentar} onChange={e => setForm({ ...form, kommentar: e.target.value })} />
              </div>
              <hr style={{ margin: '1rem 0' }} />
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                <strong>Linjer</strong>
                <button type="button" className="btn btn-sm btn-secondary" onClick={addLine}>+ Linje</button>
              </div>
              {form.linjer.map((linje, i) => (
                <div key={i} style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr', gap: '0.5rem', marginBottom: '0.5rem', alignItems: 'end' }}>
                  <div className="form-group">
                    <label>Artikkel</label>
                    <select value={linje.artikkelId} onChange={e => {
                      const a = articles.find(x => x.id === parseInt(e.target.value));
                      updateLine(i, 'artikkelId', parseInt(e.target.value));
                      if (a) { updateLine(i, 'enhet', a.enhet); }
                    }}>
                      <option value={0}>Velg...</option>
                      {articles.map(a => <option key={a.id} value={a.id}>{a.navn} ({a.artikkelNr})</option>)}
                    </select>
                  </div>
                  <div className="form-group">
                    <label>Lotnr</label>
                    <input value={linje.lotNr} onChange={e => updateLine(i, 'lotNr', e.target.value)} />
                  </div>
                  <div className="form-group">
                    <label>Mengde</label>
                    <input type="number" step="0.001" min="0" value={linje.mengde} onChange={e => updateLine(i, 'mengde', parseFloat(e.target.value))} />
                  </div>
                  <div className="form-group">
                    <label>Best-før</label>
                    <input type="date" value={linje.bestForDato} onChange={e => updateLine(i, 'bestForDato', e.target.value)} />
                  </div>
                  <button type="button" className="btn btn-sm btn-danger" onClick={() => removeLine(i)}>✕</button>
                </div>
              ))}
              {error && <div className="alert alert-error" style={{ marginTop: '0.5rem' }}>{error}</div>}
              <div className="form-actions" style={{ marginTop: '1rem' }}>
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Opprett mottak</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
