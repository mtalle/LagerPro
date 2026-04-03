'use client';
import { useState, useEffect } from 'react';
import { Bruker, getMe } from '../lib/api';

const NAV_ITEMS = [
  { href: '/', label: 'Dashboard', ressursKrav: [] },
  { href: '/artikler', label: 'Artikler', ressursKrav: [2] },
  { href: '/kunder', label: 'Kunder', ressursKrav: [8] },
  { href: '/leverandorer', label: 'Leverandører', ressursKrav: [9] },
  { href: '/lager', label: 'Lager', ressursKrav: [3] },
  { href: '/mottak', label: 'Mottak', ressursKrav: [1] },
  { href: '/produksjon', label: 'Produksjon', ressursKrav: [4] },
  { href: '/levering', label: 'Levering', ressursKrav: [6] },
  { href: '/resepter', label: 'Resepter', ressursKrav: [7] },
  { href: '/brukere', label: '👤 Admin', ressursKrav: [10] },
  { href: '/rapporter', label: '📊 Rapporter', ressursKrav: [10] },
  { href: '/sporing', label: '🧭 Sporing', ressursKrav: [7] },
];

export default function RootLayout({ children }: { children: React.ReactNode }) {
  const [menuOpen, setMenuOpen] = useState(false);
  const [bruker, setBruker] = useState<Bruker | null>(null);
  const [brukerLaster, setBrukerLaster] = useState(true);

  useEffect(() => {
    async function loadBruker() {
      try {
        const b = await getMe();
        setBruker(b);
        localStorage.setItem('lagerpro_bruker_navn', b.navn);
        localStorage.setItem('lagerpro_bruker_id', String(b.id));
      } catch {
        setBruker(null);
      } finally {
        setBrukerLaster(false);
      }
    }
    loadBruker();
  }, []);

  function kanVise(item: typeof NAV_ITEMS[number]): boolean {
    if (!bruker) return false;
    if (bruker.erAdmin) return true;
    return item.ressursKrav.every(rid => bruker.tilganger.some(t => t.ressursId === rid));
  }

  return (
    <html lang="no">
      <head>
        <meta name="viewport" content="width=device-width, initial-scale=1" />
      </head>
      <body>
        <nav className="navbar">
          <div className="nav-brand">
            <span className="logo">📦 LagerPro</span>
            <button className="hamburger" onClick={() => setMenuOpen(!menuOpen)} aria-label="Meny">
              <span className={menuOpen ? 'hamburger-icon open' : 'hamburger-icon'}></span>
            </button>
          </div>
          <div className={`nav-links ${menuOpen ? 'open' : ''}`}>
            {NAV_ITEMS.filter(kanVise).map(item => (
              <a key={item.href} href={item.href} onClick={() => setMenuOpen(false)}>{item.label}</a>
            ))}
          </div>
          {!brukerLaster && bruker && (
            <div className="nav-bruker" style={{ marginLeft: 'auto', padding: '0 1rem', fontSize: '0.85rem', color: '#6b7280' }}>
              Innlogget: <strong>{bruker.navn}</strong>
            </div>
          )}
        </nav>
        <main className="container">{children}</main>
      </body>
    </html>
  );
}
