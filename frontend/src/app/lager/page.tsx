'use client';
import { useEffect, useState } from 'react';
import { LagerBeholdning, get } from '../../lib/api';

export default function LagerPage() {
  const [beholdninger, setBeholdninger] = useState<LagerBeholdning[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [visKunLav, setVisKunLav] = useState(false);

  useEffect(() => { loadData(); }, []);

  async function loadData() {
    try {
      const data = await get<LagerBeholdning[]>('/lager');
      setBeholdninger(data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  const filtered = beholdninger.filter(b => {
    const matchesSearch =
      b.artikkelNavn.toLowerCase().includes(search.toLowerCase()) ||
      b.artikkelNr.toLowerCase().includes(search.toLowerCase()) ||
      b.lotNr.toLowerCase().includes(search.toLowerCase()) ||
      (b.lokasjon ?? '').toLowerCase().includes(search.toLowerCase());

    const erLav = b.minBeholdning != null && b.mengde < b.minBeholdning;

    return matchesSearch && (!visKunLav || erLav);
  });

  if (loading) return <div className="loading">Laster lager...</div>;

  return (
    <>
      <div className="page-header">
        <h1>🏭 Lagerbeholdning</h1>
      </div>

      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', marginBottom: '1rem', flexWrap: 'wrap' }}>
        <input
          placeholder="Søk artikkel, lotnr eller lokasjon..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '0.4rem 0.8rem', border: '1px solid #d1d5db', borderRadius: 6, width: 320, fontSize: '0.9rem' }}
        />
        <label style={{ display: 'flex', alignItems: 'center', gap: 6, fontSize: '0.9rem', cursor: 'pointer', userSelect: 'none' }}>
          <input
            type="checkbox"
            checked={visKunLav}
            onChange={e => setVisKunLav(e.target.checked)}
          />
          Vis kun lav beholdning
        </label>
      </div>

      <table>
        <thead>
          <tr>
            <th></th>
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
            <tr><td colSpan={9} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen beholdning funnet</td></tr>
          ) : filtered.map(b => {
            const erLav = b.minBeholdning != null && b.mengde < b.minBeholdning;
            return (
              <tr key={b.id} style={erLav ? { background: '#fef2f2' } : undefined}>
                <td style={{ width: 32, textAlign: 'center' }}>
                  {erLav && <span title={`Lav beholdning — minst ${b.minBeholdning} ${b.enhet}`}>⚠️</span>}
                </td>
                <td><code>{b.artikkelNr}</code></td>
                <td>{b.artikkelNavn}</td>
                <td><code>{b.lotNr}</code></td>
                <td><strong style={erLav ? { color: '#dc2626' } : undefined}>{b.mengde}</strong></td>
                <td>{b.enhet}</td>
                <td>{b.lokasjon ?? '—'}</td>
                <td>{b.bestForDato ? new Date(b.bestForDato).toLocaleDateString('no-NO') : '—'}</td>
                <td>{new Date(b.sistOppdatert).toLocaleDateString('no-NO')}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </>
  );
}
