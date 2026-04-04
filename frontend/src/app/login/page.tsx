'use client';
import { useState } from 'react';
import { loginBruker } from '../../lib/api';

export default function LoginPage() {
  const [brukernavn, setBrukernavn] = useState('');
  const [passord, setPassord] = useState('');
  const [error, setError] = useState('');

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    try {
      const b = await loginBruker(brukernavn, passord);
      localStorage.setItem('lagerpro_bruker_id', String(b.id));
      localStorage.setItem('lagerpro_bruker_navn', b.navn);
      window.location.href = '/';
    } catch (err) {
      setError('Feil brukernavn eller passord');
    }
  }

  return (
    <div style={{ maxWidth: '400px', margin: '4rem auto', padding: '2rem', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>Logg inn</h2>
      {error && <div style={{ color: 'red', marginBottom: '1rem' }}>{error}</div>}
      <form onSubmit={handleLogin} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        <div>
          <label>Brukernavn</label>
          <input type="text" value={brukernavn} onChange={(e) => setBrukernavn(e.target.value)} required style={{ width: '100%', padding: '0.5rem' }} />
        </div>
        <div>
          <label>Passord</label>
          <input type="password" value={passord} onChange={(e) => setPassord(e.target.value)} required style={{ width: '100%', padding: '0.5rem' }} />
        </div>
        <button type="submit" style={{ padding: '0.75rem', background: '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>Logg inn</button>
      </form>
    </div>
  );
}
