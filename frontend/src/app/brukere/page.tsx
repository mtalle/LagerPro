'use client';
import { useEffect, useState } from 'react';
import { Bruker, Ressurs, get, put } from '../../lib/api';

export default function BrukerePage() {
  const [brukere, setBrukere] = useState<Bruker[]>([]);
  const [ressurser, setRessurser] = useState<Ressurs[]>([]);
  const [selected, setSelected] = useState<Bruker | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  // Local tilganger state while editing
  const [selectedTilganger, setSelectedTilganger] = useState<Set<number>>(new Set());

  useEffect(() => { load(); }, []);

  async function load() {
    try {
      const [b, r] = await Promise.all([
        get<Bruker[]>('/brukere'),
        get<Ressurs[]>('/brukere/ressurser'),
      ]);
      setBrukere(b);
      setRessurser(r);
    } catch (e) {
      setError('Kunne ikke laste data: ' + (e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  function selectBruker(bruker: Bruker) {
    setSelected(bruker);
    setSelectedTilganger(new Set(bruker.tilganger.map(t => t.ressursId)));
    setSuccess('');
    setError('');
    // Sett aktiv bruker i localStorage
    localStorage.setItem('lagerpro_bruker_id', String(bruker.id));
    localStorage.setItem('lagerpro_bruker_navn', bruker.navn);
  }

  function toggleRessurs(ressursId: number) {
    const next = new Set(selectedTilganger);
    if (next.has(ressursId)) {
      next.delete(ressursId);
    } else {
      next.add(ressursId);
    }
    setSelectedTilganger(next);
  }

  async function saveTilganger() {
    if (!selected) return;
    setSaving(true);
    setError('');
    setSuccess('');
    try {
      await put(`/brukere/${selected.id}/tilganger`, { ressursIder: Array.from(selectedTilganger) });
      setSuccess('Tilganger lagret!');
      // Lagre valgt bruker i localStorage slik at API-kall får X-Bruker-Id
      localStorage.setItem('lagerpro_bruker_id', String(selected.id));
      localStorage.setItem('lagerpro_bruker_navn', selected.navn);
      // Refresh
      const oppdatert = await get<Bruker[]>(`/brukere`);
      setBrukere(oppdatert);
      const oppdatertBruker = oppdatert.find(b => b.id === selected.id);
      if (oppdatertBruker) {
        setSelected(oppdatertBruker);
        setSelectedTilganger(new Set(oppdatertBruker.tilganger.map(t => t.ressursId)));
      }
    } catch (e) {
      setError('Feil ved lagring: ' + (e as Error).message);
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <div className="loading">Laster brukere...</div>;

  return (
    <>
      <div className="page-header">
        <h1>👤 Brukeradministrasjon</h1>
      </div>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}

      <div className="split-grid">
        {/* Brukerliste */}
        <div className="card">
          <h3>Brukere</h3>
          {brukere.map(b => (
            <div
              key={b.id}
              onClick={() => selectBruker(b)}
              className="bruker-item"
              data-selected={selected?.id === b.id}
            >
              <div>
                <div style={{ fontWeight: 600, fontSize: '0.95rem' }}>{b.navn}</div>
                <div style={{ fontSize: '0.8rem', color: '#6b7280' }}>{b.brukernavn}</div>
              </div>
              <div className="flex-gap-4">
                {b.erAdmin && <span className="badge badge-registrert">Admin</span>}
                {!b.aktiv && <span className="badge badge-inactive">Inaktiv</span>}
              </div>
            </div>
          ))}
          {brukere.length === 0 && <div className="empty">Ingen brukere funnet.</div>}
        </div>

        {/* Tilganger-editor */}
        {selected ? (
          <div className="card">
            <div className="card-header">
              <div>
                <h3 className="card-title" style={{ margin: 0 }}>Tilganger for {selected.navn}</h3>
                <span style={{ fontSize: '0.8rem', color: '#6b7280' }}>
                  {selected.erAdmin ? 'Administrator – alle tilganger' : 'Vanlig bruker'}
                </span>
              </div>
              <button
                className="btn btn-primary"
                onClick={saveTilganger}
                disabled={saving || selected.erAdmin}
                style={{ minWidth: 120 }}
              >
                {saving ? 'Lagrer...' : 'Lagre tilganger'}
              </button>
            </div>

            {selected.erAdmin && (
              <div className="alert alert-success" style={{ marginBottom: '1rem' }}>
                Admin-bruker har alle tilganger automatisk.
              </div>
            )}

            {!selected.erAdmin && (
              <div className="tilgang-grid">
                {ressurser.map(r => {
                  const har = selectedTilganger.has(r.id);
                  return (
                    <div
                      key={r.id}
                      className="tilgang-item"
                      onClick={() => toggleRessurs(r.id)}
                      style={{ background: har ? '#e0e7ff' : undefined }}
                    >
                      <input
                        type="checkbox"
                        checked={har}
                        onChange={() => {}}
                        style={{ pointerEvents: 'none' }}
                      />
                      <div>
                        <div style={{ fontWeight: 500, fontSize: '0.9rem' }}>{r.navn}</div>
                        <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>{r.beskrivelse}</div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        ) : (
          <div className="card" style={{ textAlign: 'center', padding: '3rem', color: '#9ca3af' }}>
            Velg en bruker fra listen for å redigere tilganger.
          </div>
        )}
      </div>
    </>
  );
}
