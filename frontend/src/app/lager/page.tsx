'use client';
import { useEffect, useState } from 'react';
import { LagerBeholdning, get } from '../../lib/api';

export default function LagerPage() {
  const [beholdninger, setBeholdninger] = useState<LagerBeholdning[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');

  useEffect(() => { loadData(); }, []);

  async function loadData() {
    try {
      const data = await get<LagerBeholdning[]>('/inventory');
      setBeholdninger(data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  const filtered = beholdninger.filter(b =>
    b.artikkelNavn.toLowerCase().includes(search.toLowerCase()) ||
    b.artikkelNr.toLowerCase().includes(search.toLowerCase()) ||
    b.lotNr.toLowerCase().includes(search.toLowerCase()) ||
    (b.lokasjon ?? '').toLowerCase().includes(search.toLowerCase())
  );

  if (loading) return <div className="loading">Laster lager...</div>;

  return (
    <>
      <div className="page-header">
        <h1>🏭 Lagerbeholdning</h1>
      </div>

      <div style={{ marginBottom: '1rem' }}>
        <input
          placeholder="Søk artikkel, lotnr eller lokasjon..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 320, fontSize: '0.9rem' }}
        />
      </div>

      <table>
        <thead>
          <tr>
            <th>Artikkelnr</th>
            <th>Navn</th>
            <th>Lotnr</th>
            <th>Mengde</th>
            <th>Enhet</th>
            <th>Lokasjon</th>
            <th>Best-før</th>
            <th>Oppdatert</th>
          </tr>
        </thead>
        <tbody>
          {filtered.length === 0 ? (
            <tr><td colSpan={8} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen beholdning funnet</td></tr>
          ) : filtered.map(b => (
            <tr key={b.id}>
              <td><code>{b.artikkelNr}</code></td>
              <td>{b.artikkelNavn}</td>
              <td><code>{b.lotNr}</code></td>
              <td><strong>{b.mengde}</strong></td>
              <td>{b.enhet}</td>
              <td>{b.lokasjon ?? '—'}</td>
              <td>{b.bestForDato ? new Date(b.bestForDato).toLocaleDateString('no-NO') : '—'}</td>
              <td>{new Date(b.sistOppdatert).toLocaleDateString('no-NO')}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
