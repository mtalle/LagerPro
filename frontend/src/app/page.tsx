'use client';
import { useEffect, useState } from 'react';
import { Article, ProduksjonsOrdre, Mottak, Levering, LagerBeholdning, Kunde, Leverandor, Resept, get, Bruker, getMe } from '../lib/api';

interface DashboardStats {
  artikler: number;
  aktiveArtikler: number;
  kunder: number;
  leverandorer: number;
  resepter: number;
  lagerlinjer: number;
  lavBeholdning: number;
  aapneMottak: number;
  aapneProduksjon: number;
  aapneLevering: number;
  nyligeLeveringer: Levering[];
  aktiveOrdre: ProduksjonsOrdre[];
  apneMottakListe: Mottak[];
  lavBeholdningListe: LagerBeholdning[];
}

const WIDGET_CONFIGS = [
  { id: 'salg_i_dag', label: 'Dagens leveringer', description: 'Leveringer planlagt/sendt i dag', icon: '🚚', defaultVisible: true },
  { id: 'aktive_ordre', label: 'Aktive produksjonsordre', description: 'Planlagt og i produksjon', icon: '🏗', defaultVisible: true },
  { id: 'lav_beholdning', label: 'Lav beholdning', description: 'Artikler under minBeholdning', icon: '⚠️', defaultVisible: true },
  { id: 'apne_mottak', label: 'Åpne mottak', description: 'Mottak som venter behandling', icon: '📥', defaultVisible: false },
];

const LS_KEY = 'lagerpro_dashboard_widgets';

function loadVisibleWidgets(): string[] {
  if (typeof window === 'undefined') return WIDGET_CONFIGS.filter(w => w.defaultVisible).map(w => w.id);
  try {
    const stored = localStorage.getItem(LS_KEY);
    if (stored) return JSON.parse(stored);
  } catch { /* ignore */ }
  return WIDGET_CONFIGS.filter(w => w.defaultVisible).map(w => w.id);
}

function saveVisibleWidgets(ids: string[]) {
  if (typeof window === 'undefined') return;
  localStorage.setItem(LS_KEY, JSON.stringify(ids));
}

function DashboardWidget({ title, children, href, seeAll }: { title: string; children: React.ReactNode; href: string; seeAll: string }) {
  return (
    <div className="card">
      <div className="card-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '0 0 1rem 0', borderBottom: '1px solid #f3f4f6', marginBottom: '0.5rem' }}>
        <h3 className="card-title" style={{ margin: 0, fontSize: '1rem', fontWeight: 600 }}>{title}</h3>
        <a href={href} style={{ fontSize: '0.8rem', color: '#3b82f6', textDecoration: 'none' }}>{seeAll}</a>
      </div>
      <div style={{ padding: '0.5rem' }}>{children}</div>
    </div>
  );
}

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [visibleWidgets, setVisibleWidgets] = useState<string[]>([]);
  const [showWidgetPicker, setShowWidgetPicker] = useState(false);

  useEffect(() => { setVisibleWidgets(loadVisibleWidgets()); }, []);
  useEffect(() => {
    // Prøv å lasta brukar for å populera localStorage
    getMe().then(b => {
      localStorage.setItem('lagerpro_bruker_navn', b.navn);
      localStorage.setItem('lagerpro_bruker_id', String(b.id));
    }).catch(() => {
      // Ikkje logga inn enno — berre last data utan auth
    }).finally(() => load());
  }, []);

  async function load() {
    try {
      const [artikler, mottak, produksjon, levering, lager, kunder, leverandorer, resepter] = await Promise.all([
        get<Article[]>('/articles'),
        get<Mottak[]>('/mottak'),
        get<ProduksjonsOrdre[]>('/production'),
        get<Levering[]>('/levering'),
        get<LagerBeholdning[]>('/inventory'),
        get<Kunde[]>('/kunder'),
        get<Leverandor[]>('/leverandorer'),
        get<Resept[]>('/recipes'),
      ]);
      const aktiveArtikler = artikler.filter(a => a.aktiv).length;
      const lavBeholdningListe = lager.filter(b => b.minBeholdning != null && b.mengde < b.minBeholdning);
      const today = new Date().toISOString().slice(0, 10);
      const nyligeLeveringer = levering.filter(l => l.leveringsDato.startsWith(today) && l.status !== 'Kansellert');
      setStats({
        artikler: artikler.length, aktiveArtikler, kunder: kunder.length, leverandorer: leverandorer.length,
        resepter: resepter.length, lagerlinjer: lager.length, lavBeholdning: lavBeholdningListe.length,
        aapneMottak: mottak.filter(m => m.status === 'Registrert' || m.status === 'Mottatt').length,
        aapneProduksjon: produksjon.filter(o => o.status === 'Planlagt' || o.status === 'IProduksjon').length,
        aapneLevering: levering.filter(l => l.status === 'Planlagt' || l.status === 'Plukket').length,
        nyligeLeveringer,
        aktiveOrdre: produksjon.filter(o => o.status === 'Planlagt' || o.status === 'IProduksjon').slice(0, 5),
        apneMottakListe: mottak.filter(m => m.status === 'Registrert' || m.status === 'Mottatt').slice(0, 5),
        lavBeholdningListe: lavBeholdningListe.slice(0, 5),
      });
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }

  function toggleWidget(id: string) {
    const next = visibleWidgets.includes(id) ? visibleWidgets.filter(w => w !== id) : [...visibleWidgets, id];
    setVisibleWidgets(next);
    saveVisibleWidgets(next);
  }

  if (loading) return <div className="loading">Laster dashboard...</div>;
  if (!stats) return null;

  const statCards = [
    { label: 'Kunder', value: stats.kunder, sub: 'registrert', color: '#10b981', href: '/kunder' },
    { label: 'Leverandører', value: stats.leverandorer, sub: 'registrert', color: '#f59e0b', href: '/leverandorer' },
    { label: 'Resepter', value: stats.resepter, sub: 'aktive resepter', color: '#8b5cf6', href: '/resepter' },
    { label: 'Artikler', value: stats.artikler, sub: `${stats.aktiveArtikler} aktive`, color: '#3b82f6', href: '/artikler' },
    { label: 'Lagerlinjer', value: stats.lagerlinjer, sub: stats.lavBeholdning > 0 ? `${stats.lavBeholdning} lav beholdning` : 'Ingen lav beholdning', color: stats.lavBeholdning > 0 ? '#ef4444' : '#22c55e', href: '/lager' },
    { label: 'Åpne mottak', value: stats.aapneMottak, sub: 'krever handling', color: '#f59e0b', href: '/mottak' },
    { label: 'Produksjon', value: stats.aapneProduksjon, sub: 'planlagt/i produksjon', color: '#8b5cf6', href: '/produksjon' },
    { label: 'Levering', value: stats.aapneLevering, sub: 'planlagt/plukket', color: '#06b6d4', href: '/levering' },
  ];

  return (
    <>
      <div className="page-header">
        <h1>📦 LagerPro</h1>
        <button className="btn btn-secondary" onClick={() => setShowWidgetPicker(p => !p)}>
          {showWidgetPicker ? 'Lukk' : '⚙️ Velg widgets'}
        </button>
      </div>

      {showWidgetPicker && (
        <div className="card" style={{ marginBottom: '1.5rem' }}>
          <h3 style={{ marginBottom: '0.75rem' }}>Velg widgets</h3>
          <p style={{ fontSize: '0.85rem', color: '#6b7280', marginBottom: '1rem' }}>Huk av widgetene du vil se på dashboard. Valget lagres i nettleseren din.</p>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))', gap: '0.75rem' }}>
            {WIDGET_CONFIGS.map(w => (
              <label key={w.id} style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', cursor: 'pointer', padding: '0.75rem', borderRadius: 8, border: '1px solid #e5e7eb', background: visibleWidgets.includes(w.id) ? '#eff6ff' : 'white' }}>
                <input type="checkbox" checked={visibleWidgets.includes(w.id)} onChange={() => toggleWidget(w.id)} style={{ marginTop: 2, width: 16, height: 16, cursor: 'pointer' }} />
                <div>
                  <div style={{ fontWeight: 600, fontSize: '0.9rem' }}>{w.icon} {w.label}</div>
                  <div style={{ fontSize: '0.8rem', color: '#6b7280', marginTop: 2 }}>{w.description}</div>
                </div>
              </label>
            ))}
          </div>
        </div>
      )}

      <div className="stats-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '2rem' }}>
        {statCards.map(card => (
          <a key={card.label} href={card.href} style={{ textDecoration: 'none' }}>
            <div className="stat-card" style={{ background: 'white', borderRadius: 12, padding: '1.25rem', boxShadow: '0 1px 3px rgba(0,0,0,0.1)', border: '1px solid #e5e7eb' }}>
              <div style={{ fontSize: '0.8rem', color: '#6b7280', marginBottom: 4, fontWeight: 500 }}>{card.label}</div>
              <div style={{ fontSize: '2rem', fontWeight: 700, color: card.color }}>{card.value}</div>
              <div style={{ fontSize: '0.8rem', color: '#9ca3af', marginTop: 4 }}>{card.sub}</div>
            </div>
          </a>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1rem' }}>
        {visibleWidgets.includes('salg_i_dag') && (
          <DashboardWidget title="🚚 Dagens leveringer" href="/levering" seeAll="Se alle leveringer →">
            {stats.nyligeLeveringer.length === 0 ? (
              <div className="empty" style={{ textAlign: 'center', padding: '1.5rem', fontSize: '0.9rem' }}>Ingen leveringer planlagt i dag</div>
            ) : (
              <div className="table-wrapper">
              <table style={{ width: '100%', fontSize: '0.85rem' }}>
                <thead><tr style={{ textAlign: 'left', background: '#f9fafb' }}><th>Kunde</th><th>Dato</th><th>Status</th></tr></thead>
                <tbody>{stats.nyligeLeveringer.slice(0, 5).map(l => (
                  <tr key={l.id} style={{ borderBottom: '1px solid #f3f4f6' }}>
                    <td>{l.kundeNavn ?? `Kunde.ID ${l.kundeId}`}</td>
                    <td>{new Date(l.leveringsDato).toLocaleDateString('no-NO')}</td>
                    <td><span className={`badge ${l.status === 'Sendt' || l.status === 'Levert' ? 'badge-levert' : 'badge-planlagt'}`}>{l.status}</span></td>
                  </tr>
                ))}</tbody>
              </table>
              </div>
            )}
          </DashboardWidget>
        )}

        {visibleWidgets.includes('aktive_ordre') && (
          <DashboardWidget title="🏗 Aktive produksjonsordre" href="/produksjon" seeAll="Se alle ordre →">
            {stats.aktiveOrdre.length === 0 ? (
              <div className="empty" style={{ textAlign: 'center', padding: '1.5rem', fontSize: '0.9rem' }}>Ingen aktive produksjonsordre</div>
            ) : (
              <div className="table-wrapper">
              <table style={{ width: '100%', fontSize: '0.85rem' }}>
                <thead><tr style={{ textAlign: 'left', background: '#f9fafb' }}><th>OrdreNr</th><th>Resept</th><th>Status</th></tr></thead>
                <tbody>{stats.aktiveOrdre.map(o => (
                  <tr key={o.id} style={{ borderBottom: '1px solid #f3f4f6' }}>
                    <td><code>{o.ordreNr}</code></td>
                    <td>{o.reseptNavn ?? `Resept.ID ${o.reseptId}`}</td>
                    <td><span className={`badge ${o.status === 'IProduksjon' ? 'badge-i-produksjon' : 'badge-planlagt'}`}>{o.status}</span></td>
                  </tr>
                ))}</tbody>
              </table>
              </div>
            )}
          </DashboardWidget>
        )}

        {visibleWidgets.includes('lav_beholdning') && (
          <DashboardWidget title="⚠️ Lav beholdning" href="/lager" seeAll="Se alle →">
            {stats.lavBeholdningListe.length === 0 ? (
              <div className="empty" style={{ textAlign: 'center', padding: '1.5rem', fontSize: '0.9rem', color: '#22c55e' }}>✅ Alle artikler er over minBeholdning</div>
            ) : (
              <div className="table-wrapper">
              <table style={{ width: '100%', fontSize: '0.85rem' }}>
                <thead><tr style={{ textAlign: 'left', background: '#f9fafb' }}><th>Artikkel</th><th>LOT</th><th>Beholdning</th><th>Min</th></tr></thead>
                <tbody>{stats.lavBeholdningListe.map(b => (
                  <tr key={b.id} style={{ borderBottom: '1px solid #f3f4f6' }}>
                    <td>{b.artikkelNavn}</td><td><code>{b.lotNr}</code></td>
                    <td style={{ color: '#dc2626', fontWeight: 600 }}>{b.mengde} {b.enhet}</td>
                    <td style={{ color: '#6b7280' }}>{b.minBeholdning} {b.enhet}</td>
                  </tr>
                ))}</tbody>
              </table>
              </div>
            )}
          </DashboardWidget>
        )}

        {visibleWidgets.includes('apne_mottak') && (
          <DashboardWidget title="📥 Åpne mottak" href="/mottak" seeAll="Se alle mottak →">
            {stats.apneMottakListe.length === 0 ? (
              <div className="empty" style={{ textAlign: 'center', padding: '1.5rem', fontSize: '0.9rem', color: '#22c55e' }}>✅ Ingen åpne mottak</div>
            ) : (
              <div className="table-wrapper">
              <table style={{ width: '100%', fontSize: '0.85rem' }}>
                <thead><tr style={{ textAlign: 'left', background: '#f9fafb' }}><th>Leverandør</th><th>Dato</th><th>Status</th></tr></thead>
                <tbody>{stats.apneMottakListe.map(m => (
                  <tr key={m.id} style={{ borderBottom: '1px solid #f3f4f6' }}>
                    <td>{m.leverandorNavn ?? `Lev.ID ${m.leverandorId}`}</td>
                    <td>{new Date(m.mottaksDato).toLocaleDateString('no-NO')}</td>
                    <td><span className={`badge ${m.status === 'Mottatt' ? 'badge-aktiv' : 'badge-registrert'}`}>{m.status}</span></td>
                  </tr>
                ))}</tbody>
              </table>
              </div>
            )}
          </DashboardWidget>
        )}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '1rem', marginTop: '1.5rem' }}>
        <QuickAction title="📥 Nytt mottak" href="/mottak" desc="Registrer inncoming varer med LOT-sporing" />
        <QuickAction title="🏗 Ny produksjon" href="/produksjon" desc="Opprett produksjonsordre fra resept" />
        <QuickAction title="🚚 Ny levering" href="/levering" desc="Send ferdigvare til kunde med sporing" />
        <QuickAction title="📦 Artikler" href="/artikler" desc="Administrer varelager og priser" />
        <QuickAction title="👥 Kunder" href="/kunder" desc="Administrer kunderegister" />
        <QuickAction title="🏭 Leverandører" href="/leverandorer" desc="Administrer leverandører" />
      </div>
    </>
  );
}

function QuickAction({ title, href, desc }: { title: string; href: string; desc: string }) {
  return (
    <a href={href} style={{ textDecoration: 'none' }}>
      <div className="card" style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '1rem 1.25rem' }}>
        <div>
          <div style={{ fontWeight: 600, color: '#111827' }}>{title}</div>
          <div style={{ fontSize: '0.8rem', color: '#6b7280', marginTop: 2 }}>{desc}</div>
        </div>
      </div>
    </a>
  );
}