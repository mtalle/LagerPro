'use client';
import { useEffect, useState } from 'react';
import { Article, ProduksjonsOrdre, Mottak, Levering, LagerBeholdning, Kunde, Leverandor, Resept, get } from '../lib/api';

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
}

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => { load(); }, []);

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
      const lavBeholdning = lager.filter(b => b.minBeholdning != null && b.mengde < b.minBeholdning).length;

      setStats({
        artikler: artikler.length,
        aktiveArtikler,
        kunder: kunder.length,
        leverandorer: leverandorer.length,
        resepter: resepter.length,
        lagerlinjer: lager.length,
        lavBeholdning,
        aapneMottak: mottak.filter(m => m.status === 'Registrert' || m.status === 'Mottatt').length,
        aapneProduksjon: produksjon.filter(o => o.status === 'Planlagt' || o.status === 'IProduksjon').length,
        aapneLevering: levering.filter(l => l.status === 'Planlagt' || l.status === 'Plukket').length,
      });
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  if (loading) return <div className="loading">Laster dashboard...</div>;
  if (!stats) return null;

  const statCards = [
    { label: 'Kunder', value: stats.kunder, sub: 'registrert', color: '#10b981', href: '/kunder' },
    { label: 'Leverandører', value: stats.leverandorer, sub: 'registrert', color: '#f59e0b', href: '/leverandorer' },
    { label: 'Resepter', value: stats.resepter, sub: 'aktive resepter', color: '#8b5cf6', href: '/resepter' },
    { label: 'Artikler', value: stats.artikler, sub: `${stats.aktiveArtikler} aktive`, color: '#3b82f6', href: '/artikler' },
    { label: 'Lagerlinjer', value: stats.lagerlinjer, sub: stats.lavBeholdning > 0 ? `${stats.lavBeholdning} lav beholdning` : ' Ingen lav beholdning', color: stats.lavBeholdning > 0 ? '#ef4444' : '#22c55e', href: '/lager' },
    { label: 'Åpne mottak', value: stats.aapneMottak, sub: 'krever handling', color: '#f59e0b', href: '/mottak' },
    { label: 'Produksjon', value: stats.aapneProduksjon, sub: 'planlagt/i produksjon', color: '#8b5cf6', href: '/produksjon' },
    { label: 'Levering', value: stats.aapneLevering, sub: 'planlagt/plukket', color: '#06b6d4', href: '/levering' },
  ];

  return (
    <>
      <div className="page-header">
        <h1>📦 LagerPro</h1>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '2rem' }}>
        {statCards.map(card => (
          <a key={card.label} href={card.href} style={{ textDecoration: 'none' }}>
            <div style={{
              background: 'white',
              borderRadius: 12,
              padding: '1.25rem',
              boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
              border: '1px solid #e5e7eb',
            }}>
              <div style={{ fontSize: '0.8rem', color: '#6b7280', marginBottom: 4, fontWeight: 500 }}>{card.label}</div>
              <div style={{ fontSize: '2rem', fontWeight: 700, color: card.color }}>{card.value}</div>
              <div style={{ fontSize: '0.8rem', color: '#9ca3af', marginTop: 4 }}>{card.sub}</div>
            </div>
          </a>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '1rem' }}>
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
      <div style={{
        background: 'white',
        borderRadius: 12,
        padding: '1rem 1.25rem',
        boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
        border: '1px solid #e5e7eb',
        display: 'flex',
        alignItems: 'center',
        gap: '0.75rem',
      }}>
        <div>
          <div style={{ fontWeight: 600, color: '#111827' }}>{title}</div>
          <div style={{ fontSize: '0.8rem', color: '#6b7280', marginTop: 2 }}>{desc}</div>
        </div>
      </div>
    </a>
  );
}
