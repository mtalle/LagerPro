'use client';
import { useEffect, useState } from 'react';
import { Levering, Kunde, LagerBeholdning, get, post, patch } from '../../lib/api';

const STATUS_MAP: Record<string, string> = {
  Planlagt: 'badge-planlagt',
  Plukket: 'badge-planlagt',
  Sendt: 'badge-sendt',
  Levert: 'badge-levert',
  Kansellert: 'badge-kansellert',
};

export default function LeveringPage() {
  const [leveringer, setLeveringer] = useState<Levering[]>([]);
  const [kunder, setKunder] = useState<Kunde[]>([]);
  const [lager, setLager] = useState<LagerBeholdning[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [expanded, setExpanded] = useState<number | null>(null);
  const [showModal, setShowModal] = useState(false);

  const [form, setForm] = useState({
    kundeId: 0, leveringsDato: new Date().toISOString().slice(0, 10),
    referanse: '', fraktBrev: '', kommentar: '',
    linjer: [{ artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK' }],
  });

  useEffect(() => { load(); }, []);
  async function load() {
    try {
      const [l, k, b] = await Promise.all([
        get<Levering[]>('/levering'),
        get<Kunde[]>('/kunder'),
        get<LagerBeholdning[]>('/lager'),
      ]);
      setLeveringer(l);
      setKunder(k);
      setLager(b);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  function addLine() {
    setForm({ ...form, linjer: [...form.linjer, { artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK' }] });
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
    if (form.kundeId === 0) { alert('Velg en kunde.'); return; }
    if (form.linjer.some(l => l.artikkelId === 0 || !l.lotNr)) { alert('Alle linjer må ha artikkel og lotnr.'); return; }
    try {
      await post('/levering', {
        ...form,
        leveringsDato: new Date(form.leveringsDato).toISOString(),
        linjer: form.linjer.map(l => ({ artikkelId: l.artikkelId, lotNr: l.lotNr, mengde: l.mengde, enhet: l.enhet })),
      });
      setShowModal(false);
      resetForm();
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  function resetForm() {
    setForm({ kundeId: 0, leveringsDato: new Date().toISOString().slice(0, 10), referanse: '', fraktBrev: '', kommentar: '', linjer: [{ artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK' }] });
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/levering/${id}/status`, { status }); load(); }
    catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  function getAvailableLots(artikkelId: number) {
    return lager.filter(b => b.artikkelId === artikkelId && b.mengde > 0);
  }

  function artikkelNavn(id: number) {
    const b = lager.find(x => x.artikkelId === id);
    return b ? `${b.artikkelNavn} (${b.artikkelNr})` : `Art.ID ${id}`;
  }

  if (loading) return <div className="loading">Laster leveringer...</div>;

  const filtered = leveringer.filter(l => {
    const q = search.toLowerCase();
    return (
      (l.kundeNavn ?? '').toLowerCase().includes(q) ||
      (l.referanse ?? '').toLowerCase().includes(q) ||
      (l.fraktBrev ?? '').toLowerCase().includes(q) ||
      l.id.toString().includes(q)
    );
  });

  return (
    <>
      <div className="page-header">
        <h1>🚚 Levering</h1>
        <button className="btn btn-primary" onClick={() => setShowModal(true)}>+ Ny levering</button>
      </div>

      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', marginBottom: '1rem' }}>
        <input
          placeholder="Søk kunde, referanse, fraktbrev..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 300, fontSize: '0.9rem' }}
        />
      </div>

      <table>
        <thead>
          <tr><th>ID</th><th>Dato</th><th>Kunde</th><th>Referanse</th><th>Fraktbrev</th><th>Status</th><th></th></tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={7} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>{leveringer.length === 0 ? 'Ingen leveringer' : 'Ingen resultater'}</td></tr>
          ) : filtered.map(l => (
            <tr key={l.id} style={{ cursor: 'pointer' }} onClick={() => setExpanded(expanded === l.id ? null : l.id)}>
              <td>#{l.id}</td>
              <td>{new Date(l.leveringsDato).toLocaleDateString('no-NO')}</td>
              <td>{l.kundeNavn ?? `Kunde.ID ${l.kundeId}`}</td>
              <td>{l.referanse ?? '—'}</td>
              <td>{l.fraktBrev ?? '—'}</td>
              <td><span className={`badge ${STATUS_MAP[l.status] ?? ''}`}>{l.status}</span></td>
              <td onClick={e => e.stopPropagation()}>
                {l.status === 'Planlagt' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(l.id, 'Plukket')}>Plukket</button>}
                {l.status === 'Plukket' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(l.id, 'Sendt')}>Sendt</button>}
                {l.status === 'Sendt' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(l.id, 'Levert')}>Levert</button>}
                {l.status !== 'Levert' && l.status !== 'Kansellert' && <button className="btn btn-sm btn-danger" onClick={() => setStatus(l.id, 'Kansellert')}>Kanseller</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()} style={{ maxWidth: 700 }}>
            <h2>Ny levering</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Kunde *</label>
                  <select required value={form.kundeId} onChange={e => setForm({ ...form, kundeId: parseInt(e.target.value) })}>
                    <option value={0}>Velg kunde...</option>
                    {kunder.map(k => <option key={k.id} value={k.id}>{k.navn}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Leveringsdato</label>
                  <input type="date" value={form.leveringsDato} onChange={e => setForm({ ...form, leveringsDato: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Referanse</label>
                  <input value={form.referanse} onChange={e => setForm({ ...form, referanse: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Fraktbrev</label>
                  <input value={form.fraktBrev} onChange={e => setForm({ ...form, fraktBrev: e.target.value })} />
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
              {form.linjer.map((linje, i) => {
                const artikkelId = linje.artikkelId;
                const availableLots = artikkelId > 0 ? getAvailableLots(artikkelId) : [];
                return (
                  <div key={i} style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr auto', gap: '0.5rem', marginBottom: '0.5rem', alignItems: 'end' }}>
                    <div className="form-group">
                      <label>Artikkel</label>
                      <select value={linje.artikkelId} onChange={e => {
                        updateLine(i, 'artikkelId', parseInt(e.target.value));
                        updateLine(i, 'lotNr', '');
                      }}>
                        <option value={0}>Velg...</option>
                        {lager.map(b => <option key={`${b.artikkelId}-${b.lotNr}`} value={b.artikkelId}>{b.artikkelNavn} ({b.artikkelNr}) - {b.lotNr} ({b.mengde} {b.enhet})</option>)}
                      </select>
                    </div>
                    <div className="form-group">
                      <label>Lotnr</label>
                      <select value={linje.lotNr} onChange={e => updateLine(i, 'lotNr', e.target.value)}>
                        <option value="">Velg lot...</option>
                        {availableLots.map(b => <option key={b.lotNr} value={b.lotNr}>{b.lotNr} ({b.mengde} {b.enhet})</option>)}
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
                    <button type="button" className="btn btn-sm btn-danger" onClick={() => removeLine(i)}>✕</button>
                  </div>
                );
              })}
              <div className="form-actions" style={{ marginTop: '1rem' }}>
                <button type="button" className="btn btn-secondary" onClick={() => { setShowModal(false); resetForm(); }}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Opprett levering</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
