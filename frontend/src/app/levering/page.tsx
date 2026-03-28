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
  const [beholdning, setBeholdning] = useState<LagerBeholdning[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [expanded, setExpanded] = useState<number | null>(null);
  const [showModal, setShowModal] = useState(false);

  const [form, setForm] = useState({
    kundeId: 0, leveringsDato: new Date().toISOString().slice(0, 10),
    referanse: '', fraktBrev: '', kommentar: '', levertAv: '',
    linjer: [{ artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK' }],
  });

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const [l, k, b] = await Promise.all([
        get<Levering[]>('/levering'),
        get<Kunde[]>('/kunder'),
        get<LagerBeholdning[]>('/inventory'),
      ]);
      setLeveringer(l);
      setKunder(k);
      setBeholdning(b);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  function addLine() {
    setForm({ ...form, linjer: [...form.linjer, { artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK' }] });
  }

  function updateLine(i: number, field: string, value: string | number) {
    const lines = [...form.linjer];
    (lines[i] as Record<string, string | number>)[field] = value;
    // Auto-fill enhet when lotNr selected
    if (field === 'lotNr' && value) {
      const bh = beholdning.find(b => b.lotNr === value);
      if (bh) {
        lines[i].artikkelId = bh.artikkelId;
        lines[i].enhet = bh.enhet;
      }
    }
    setForm({ ...form, linjer: lines });
  }

  function removeLine(i: number) {
    setForm({ ...form, linjer: form.linjer.filter((_, idx) => idx !== i) });
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    try {
      await post('/levering', {
        ...form,
        leveringsDato: new Date(form.leveringsDato).toISOString(),
      });
      setShowModal(false);
      setForm({ kundeId: 0, leveringsDato: new Date().toISOString().slice(0, 10), referanse: '', fraktBrev: '', kommentar: '', levertAv: '', linjer: [{ artikkelId: 0, lotNr: '', mengde: 1, enhet: 'STK' }] });
      load();
    } catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/levering/${id}/status`, { status }); load(); }
    catch (e) { alert('Feil: ' + (e as Error).message); }
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
            <tbody key={l.id}>
              <tr style={{ cursor: 'pointer' }} onClick={() => setExpanded(expanded === l.id ? null : l.id)}>
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
              {expanded === l.id && l.linjer.map(linje => (
                <tr key={linje.id} style={{ background: '#f9fafb', fontSize: '0.85rem' }}>
                  <td colSpan={3}></td>
                  <td><code>{linje.artikkelNavn ?? `Art.ID ${linje.artikkelId}`}</code></td>
                  <td>Lot: <code>{linje.lotNr}</code></td>
                  <td>{linje.mengde} {linje.enhet}</td>
                  <td></td>
                </tr>
              ))}
            </tbody>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>Ny levering</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Kunde *</label>
                  <select required value={form.kundeId} onChange={e => setForm({ ...form, kundeId: parseInt(e.target.value) || 0 })}>
                    <option value={0}>Velg kunde...</option>
                    {kunder.filter(k => k.aktiv).map(k => <option key={k.id} value={k.id}>{k.navn}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label>Leveringsdato *</label>
                  <input type="date" required value={form.leveringsDato} onChange={e => setForm({ ...form, leveringsDato: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Referanse</label>
                  <input value={form.referanse} onChange={e => setForm({ ...form, referanse: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Fraktbrev</label>
                  <input value={form.fraktBrev} onChange={e => setForm({ ...form, fraktBrev: e.target.value })} />
                </div>
                <div className="form-group">
                  <label>Levert av</label>
                  <input value={form.levertAv} onChange={e => setForm({ ...form, levertAv: e.target.value })} />
                </div>
              </div>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label>Kommentar</label>
                <textarea rows={2} value={form.kommentar} onChange={e => setForm({ ...form, kommentar: e.target.value })} />
              </div>

              <div style={{ marginBottom: '0.5rem', fontWeight: 600, fontSize: '0.9rem', color: '#374151' }}>Linjer</div>
              <table style={{ marginBottom: '1rem' }}>
                <thead>
                  <tr><th>Artikkel/LOT</th><th>Mengde</th><th>Enhet</th><th></th></tr>
                </thead>
                <tbody>
                  {form.linjer.map((linje, i) => (
                    <tr key={i}>
                      <td>
                        <select required value={linje.lotNr} onChange={e => updateLine(i, 'lotNr', e.target.value)} style={{ minWidth: 200 }}>
                          <option value="">Velg LOT...</option>
                          {beholdning.map(b => <option key={b.id} value={b.lotNr}>{b.artikkelNavn} ({b.artikkelNr}) — LOT: {b.lotNr}</option>)}
                        </select>
                      </td>
                      <td><input type="number" required min="0.01" step="0.01" value={linje.mengde} onChange={e => updateLine(i, 'mengde', parseFloat(e.target.value) || 0)} style={{ width: 80 }} /></td>
                      <td><input value={linje.enhet} onChange={e => updateLine(i, 'enhet', e.target.value)} style={{ width: 60 }} /></td>
                      <td><button type="button" className="btn btn-sm btn-danger" onClick={() => removeLine(i)}>×</button></td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <button type="button" className="btn btn-secondary" style={{ marginBottom: '1rem' }} onClick={addLine}>+ Legg til linje</button>

              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)}>Avbryt</button>
                <button type="submit" className="btn btn-primary">Opprett levering</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
