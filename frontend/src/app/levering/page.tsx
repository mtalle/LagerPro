'use client';
import { useEffect, useState } from 'react';
import { Levering, get, patch } from '../../lib/api';

const STATUS_MAP: Record<string, string> = {
  Registrert: 'badge-registrert',
  Plukket: 'badge-planlagt',
  Sendt: 'badge-sendt',
  Levert: 'badge-levert',
  Kansellert: 'badge-kansellert',
};

export default function LeveringPage() {
  const [leveringer, setLeveringer] = useState<Levering[]>([]);
  const [loading, setLoading] = useState(true);
  const [expanded, setExpanded] = useState<number | null>(null);

  useEffect(() => { load(); }, []);
  async function load() {
    try { setLeveringer(await get<Levering[]>('/shipping')); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/shipping/${id}/status`, { status }); load(); }
    catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  if (loading) return <div className="loading">Laster leveringer...</div>;

  return (
    <>
      <div className="page-header"><h1>🚚 Levering</h1></div>
      <table>
        <thead>
          <tr><th>ID</th><th>Dato</th><th>Kunde</th><th>Referanse</th><th>Fraktbrev</th><th>Status</th><th></th></tr>
        </thead>
        <tbody>
          {leveringer.length === 0 ? (
            <tr><td colSpan={7} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen leveringer</td></tr>
          ) : leveringer.map(l => (
            <>
              <tr key={l.id} style={{ cursor: 'pointer' }} onClick={() => setExpanded(expanded === l.id ? null : l.id)}>
                <td>#{l.id}</td>
                <td>{new Date(l.leveringsDato).toLocaleDateString('no-NO')}</td>
                <td>{l.kundeNavn ?? `Kunde.ID ${l.kundeId}`}</td>
                <td>{l.referanse ?? '—'}</td>
                <td>{l.fraktBrev ?? '—'}</td>
                <td><span className={`badge ${STATUS_MAP[l.status] ?? ''}`}>{l.status}</span></td>
                <td onClick={e => e.stopPropagation()}>
                  {l.status === 'Registrert' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(l.id, 'Plukket')}>Plukket</button>}
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
            </>
          ))}
        </tbody>
      </table>
    </>
  );
}
