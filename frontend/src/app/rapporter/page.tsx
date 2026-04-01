'use client';
import { useEffect, useState } from 'react';
import { get, Lagrerapport, SalgsrapportArtikkelGruppe, SalgsrapportKundeGruppe } from '../../lib/api';

type Tab = 'lager' | 'salg-artikkel' | 'salg-kunde';

export default function RapporterPage() {
  const [activeTab, setActiveTab] = useState<Tab>('lager');
  const [lagerRapport, setLagerRapport] = useState<Lagrerapport | null>(null);
  const [salgArtikler, setSalgArtikler] = useState<SalgsrapportArtikkelGruppe | null>(null);
  const [salgKunder, setSalgKunder] = useState<SalgsrapportKundeGruppe | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [fraDato, setFraDato] = useState(() => {
    const d = new Date();
    d.setDate(d.getDate() - 30);
    return d.toISOString().split('T')[0];
  });
  const [tilDato, setTilDato] = useState(() => new Date().toISOString().split('T')[0]);

  useEffect(() => { loadData(); }, []);

  async function loadData() {
    setLoading(true);
    setError('');
    try {
      const [lag, salgA, salgK] = await Promise.all([
        get<Lagrerapport>('/rapporter/lager'),
        get<SalgsrapportArtikkelGruppe>('/rapporter/salg/artikkel?fraDato=' + fraDato + '&tilDato=' + tilDato),
        get<SalgsrapportKundeGruppe>('/rapporter/salg/kunde?fraDato=' + fraDato + '&tilDato=' + tilDato),
      ]);
      setLagerRapport(lag);
      setSalgArtikler(salgA);
      setSalgKunder(salgK);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  async function applyFilter() {
    setLoading(true);
    setError('');
    try {
      const [salgA, salgK] = await Promise.all([
        get<SalgsrapportArtikkelGruppe>('/rapporter/salg/artikkel?fraDato=' + fraDato + '&tilDato=' + tilDato),
        get<SalgsrapportKundeGruppe>('/rapporter/salg/kunde?fraDato=' + fraDato + '&tilDato=' + tilDato),
      ]);
      setSalgArtikler(salgA);
      setSalgKunder(salgK);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  function fmt(n: number) {
    return n.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  function fmtDato(d: string) {
    return new Date(d).toLocaleDateString('nb-NO');
  }

  return (
    <>
      <div className="page-header">
        <h1>Rapporter</h1>
      </div>

      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', flexWrap: 'wrap' }}>
        <button onClick={() => setActiveTab('lager')} className={'btn ' + (activeTab === 'lager' ? 'btn-primary' : 'btn-secondary')}>Lagrerapport</button>
        <button onClick={() => setActiveTab('salg-artikkel')} className={'btn ' + (activeTab === 'salg-artikkel' ? 'btn-primary' : 'btn-secondary')}>Salg per artikkel</button>
        <button onClick={() => setActiveTab('salg-kunde')} className={'btn ' + (activeTab === 'salg-kunde' ? 'btn-primary' : 'btn-secondary')}>Salg per kunde</button>
      </div>

      {error && <div className="alert alert-error" style={{ marginBottom: '1rem' }}>{error}</div>}

      {activeTab === 'lager' && <LagrerapportView loading={loading} lagerRapport={lagerRapport} fmt={fmt} fmtDato={fmtDato} />}
      {activeTab === 'salg-artikkel' && <SalgArtikkelView loading={loading} rapport={salgArtikler} fraDato={fraDato} tilDato={tilDato} setFraDato={setFraDato} setTilDato={setTilDato} onApply={applyFilter} fmt={fmt} fmtDato={fmtDato} />}
      {activeTab === 'salg-kunde' && <SalgKundeView loading={loading} rapport={salgKunder} fraDato={fraDato} tilDato={tilDato} setFraDato={setFraDato} setTilDato={setTilDato} onApply={applyFilter} fmt={fmt} fmtDato={fmtDato} />}
    </>
  );
}

function LagrerapportView({ loading, lagerRapport, fmt, fmtDato }: {
  loading: boolean; lagerRapport: Lagrerapport | null;
  fmt: (n: number) => string; fmtDato: (d: string) => string;
}) {
  if (loading) return <div className="loading">Laster lagrerapport...</div>;
  if (!lagerRapport) return null;
  const kritiske = lagerRapport.artikler.filter(a => a.kritisk).length;

  return (
    <>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '2rem' }}>
        <StatCard label="Artikler" value={lagerRapport.antallArtikler} />
        <StatCard label="Total lagerverdi" value={"kr " + fmt(lagerRapport.totalLagerverdi)} valueColor="#16a34a" />
        <StatCard label="Kritiske" value={kritiske} valueColor={kritiske > 0 ? '#dc2626' : '#16a34a'} />
        <StatCard label="Generert" value={fmtDato(lagerRapport.generert)} />
      </div>
      <table className="table-scroll">
        <thead>
          <tr><th></th><th>Artikkelnr</th><th>Navn</th><th>Beholdning</th><th>Enhet</th><th>Innpris</th><th>Totalverdi</th><th>LOT</th><th>Min</th></tr>
        </thead>
        <tbody>
          {lagerRapport.artikler.map(a => (
            <tr key={a.artikkelId} style={a.kritisk ? { background: '#fef2f2' } : undefined}>
              <td style={{ width: 32, textAlign: 'center' }}>{a.kritisk ? '⚠️' : ''}</td>
              <td><code>{a.artikkelNr}</code></td>
              <td>{a.artikkelNavn}</td>
              <td><strong style={a.kritisk ? { color: '#dc2626' } : undefined}>{a.totalMengde}</strong></td>
              <td>{a.enhet}</td>
              <td>kr {fmt(a.innpris)}</td>
              <td>kr {fmt(a.totalVerdi)}</td>
              <td>{a.antallLots}</td>
              <td style={{ color: a.kritisk ? '#dc2626' : '#9ca3af', fontSize: '0.85rem' }}>{a.minBeholdning ?? '—'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

function SalgArtikkelView({ loading, rapport, fraDato, tilDato, setFraDato, setTilDato, onApply, fmt, fmtDato }: {
  loading: boolean; rapport: SalgsrapportArtikkelGruppe | null;
  fraDato: string; tilDato: string;
  setFraDato: (v: string) => void; setTilDato: (v: string) => void;
  onApply: () => void; fmt: (n: number) => string; fmtDato: (d: string) => string;
}) {
  return (
    <>
      <DateFilter fraDato={fraDato} tilDato={tilDato} setFraDato={setFraDato} setTilDato={setTilDato} onApply={onApply} />
      {loading ? <div className="loading">Laster...</div> : rapport ? (
        <>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '2rem' }}>
            <StatCard label="Artikler solgt" value={rapport.antallArtikler} />
            <StatCard label="Leveringar" value={rapport.totaltAntallLeveringer} />
            <StatCard label="Periode" value={fmtDato(rapport.fraDato) + ' – ' + fmtDato(rapport.tilDato)} />
          </div>
          <table className="table-scroll">
            <thead>
              <tr><th>Artikkelnr</th><th>Navn</th><th>Mengde</th><th>Enhet</th><th>Leveringar</th><th>Innpris</th><th>Utpris</th></tr>
            </thead>
            <tbody>
              {rapport.artikler.length === 0 ? (
                <tr><td colSpan={7} style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen leveringar i perioden</td></tr>
              ) : rapport.artikler.map(a => (
                <tr key={a.artikkelId}>
                  <td><code>{a.artikkelNr}</code></td>
                  <td>{a.artikkelNavn}</td>
                  <td><strong>{a.totalMengde}</strong></td>
                  <td>{a.enhet}</td>
                  <td>{a.antallLeveringer}</td>
                  <td>{a.sisteInnpris != null ? 'kr ' + fmt(a.sisteInnpris) : '—'}</td>
                  <td>{a.sisteUtpris != null ? 'kr ' + fmt(a.sisteUtpris) : '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      ) : null}
    </>
  );
}

function SalgKundeView({ loading, rapport, fraDato, tilDato, setFraDato, setTilDato, onApply, fmt, fmtDato }: {
  loading: boolean; rapport: SalgsrapportKundeGruppe | null;
  fraDato: string; tilDato: string;
  setFraDato: (v: string) => void; setTilDato: (v: string) => void;
  onApply: () => void; fmt: (n: number) => string; fmtDato: (d: string) => string;
}) {
  return (
    <>
      <DateFilter fraDato={fraDato} tilDato={tilDato} setFraDato={setFraDato} setTilDato={setTilDato} onApply={onApply} />
      {loading ? <div className="loading">Laster...</div> : rapport ? (
        <>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '2rem' }}>
            <StatCard label="Kunder" value={rapport.antallKunder} />
            <StatCard label="Leveringar" value={rapport.totaltAntallLeveringer} />
            <StatCard label="Periode" value={fmtDato(rapport.fraDato) + ' – ' + fmtDato(rapport.tilDato)} />
          </div>
          {rapport.kunder.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#9ca3af', padding: '2rem' }}>Ingen leveringar i perioden</div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
              {rapport.kunder.map(k => (
                <details key={k.kundeId} style={{ background: '#fff', border: '1px solid #e5e7eb', borderRadius: 8, overflow: 'hidden' }}>
                  <summary style={{ padding: '1rem 1.25rem', cursor: 'pointer', fontWeight: 600, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <span>{k.kundeNavn} {k.orgNr ? '(' + k.orgNr + ')' : ''}</span>
                    <span style={{ color: '#6b7280', fontSize: '0.85rem', fontWeight: 400 }}>
                      {k.antallLeveringer} leveringar — {k.totalMengde} enheter
                    </span>
                  </summary>
                  <table style={{ width: '100%', borderTop: '1px solid #e5e7eb' }}>
                    <thead>
                      <tr style={{ background: '#f9fafb' }}>
                        <th style={{ padding: '0.5rem 1rem', textAlign: 'left', fontSize: '0.8rem', color: '#6b7280' }}>Dato</th>
                        <th style={{ padding: '0.5rem 1rem', textAlign: 'left', fontSize: '0.8rem', color: '#6b7280' }}>Referanse</th>
                        <th style={{ padding: '0.5rem 1rem', textAlign: 'left', fontSize: '0.8rem', color: '#6b7280' }}>Status</th>
                        <th style={{ padding: '0.5rem 1rem', textAlign: 'right', fontSize: '0.8rem', color: '#6b7280' }}>Mengde</th>
                      </tr>
                    </thead>
                    <tbody>
                      {k.leveringer.map(l => (
                        <tr key={l.leveringId} style={{ borderTop: '1px solid #f3f4f6' }}>
                          <td style={{ padding: '0.5rem 1rem', fontSize: '0.9rem' }}>{fmtDato(l.leveringsDato)}</td>
                          <td style={{ padding: '0.5rem 1rem', fontSize: '0.9rem' }}>{l.referanse || '—'}</td>
                          <td style={{ padding: '0.5rem 1rem', fontSize: '0.85rem' }}>
                            <span style={{ background: l.status === 'Ferdig' ? '#dcfce7' : l.status === 'Kansellert' ? '#fee2e2' : '#f3f4f6', padding: '2px 8px', borderRadius: 4, fontSize: '0.8rem' }}>{l.status}</span>
                          </td>
                          <td style={{ padding: '0.5rem 1rem', textAlign: 'right', fontWeight: 600 }}>{l.totalMengde}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </details>
              ))}
            </div>
          )}
        </>
      ) : null}
    </>
  );
}

function StatCard({ label, value, valueColor }: { label: string; value: string | number; valueColor?: string }) {
  return (
    <div style={{ background: '#fff', border: '1px solid #e5e7eb', borderRadius: 8, padding: '1.25rem' }}>
      <div style={{ fontSize: '0.8rem', color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{label}</div>
      <div style={{ fontSize: '1.5rem', fontWeight: 700, color: valueColor || '#1a1a2e', marginTop: '0.25rem' }}>{value}</div>
    </div>
  );
}

function DateFilter({ fraDato, tilDato, setFraDato, setTilDato, onApply }: {
  fraDato: string; tilDato: string;
  setFraDato: (v: string) => void; setTilDato: (v: string) => void; onApply: () => void;
}) {
  return (
    <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap' }}>
      <label style={{ fontSize: '0.9rem', color: '#374151' }}>Fra:
        <input type="date" value={fraDato} onChange={e => setFraDato(e.target.value)} style={{ marginLeft: '0.5rem', padding: '0.35rem 0.6rem', border: '1px solid #d1d5db', borderRadius: 6, fontSize: '0.9rem' }} />
      </label>
      <label style={{ fontSize: '0.9rem', color: '#374151' }}>Til:
        <input type="date" value={tilDato} onChange={e => setTilDato(e.target.value)} style={{ marginLeft: '0.5rem', padding: '0.35rem 0.6rem', border: '1px solid #d1d5db', borderRadius: 6, fontSize: '0.9rem' }} />
      </label>
      <button onClick={onApply} className="btn btn-primary">Oppdater</button>
    </div>
  );
}
