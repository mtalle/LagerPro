'use client';
import { useEffect, useState } from 'react';
import { ProduksjonsOrdre, get, patch } from '../../lib/api';

const STATUS_MAP: Record<string, string> = {
  Planlagt: 'badge-planlagt',
  IProduksjon: 'badge-i-produksjon',
  Ferdigmeldt: 'badge-ferdigmeldt',
  Kansellert: 'badge-kansellert',
};

export default function ProduksjonPage() {
  const [ordre, setOrdre] = useState<ProduksjonsOrdre[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => { load(); }, []);
  async function load() {
    try { setOrdre(await get<ProduksjonsOrdre[]>('/production')); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  async function setStatus(id: number, status: string) {
    try { await patch(`/production/${id}/status`, { status }); load(); }
    catch (e) { alert('Feil: ' + (e as Error).message); }
  }

  if (loading) return <div className="loading">Laster produksjonsordrer...</div>;

  return (
    <>
      <div className="page-header"><h1>🏗 Produksjon</h1></div>
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
                {o.status === 'IProduksjon' && <button className="btn btn-sm btn-primary" style={{ marginRight: 4 }} onClick={() => setStatus(o.id, 'Ferdigmeldt')}>Ferdigmeld</button>}
                {o.status !== 'Ferdigmeldt' && o.status !== 'Kansellert' && <button className="btn btn-sm btn-danger" onClick={() => setStatus(o.id, 'Kansellert')}>Kanseller</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
