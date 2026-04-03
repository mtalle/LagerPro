'use client';
import './globals.css';
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
  { href: '/sporing', label: '🧭 Sporing', ressursKrav: [] },
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

  const synligeLenker = NAV_ITEMS.filter(kanVise);

  return (
    <html lang="no">
      <head>
        <meta name="viewport" content="width=device-width, initial-scale=1" />
      </head>
      <body>
        <nav className="navbar">
          <div className="nav-inner">
            <div className="nav-brand">
              <span className="nav-brand-icon">📦</span>
              <span className="logo">LagerPro</span>
            </div>
            
            <div className="nav-links">
              {synligeLenker.map(item => (
                <a key={item.href} href={item.href}>{item.label}</a>
              ))}
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
              {!brukerLaster && bruker && (
                <div className="nav-user">
                  Innlogget: <strong>{bruker.navn}</strong>
                </div>
              )}
              
              <button className="hamburger" onClick={() => setMenuOpen(!menuOpen)} aria-label="Meny">
                <span className={menuOpen ? 'hamburger-icon open' : 'hamburger-icon'}></span>
              </button>
            </div>
          </div>
          
          <div className={`mobile-menu ${menuOpen ? 'open' : ''}`}>
            {synligeLenker.map(item => (
              <a key={item.href} href={item.href} onClick={() => setMenuOpen(false)}>{item.label}</a>
            ))}
            {!brukerLaster && bruker && (
              <div className="nav-user-mobile">
                Innlogget som <strong>{bruker.navn}</strong>
              </div>
            )}
          </div>
        </nav>
        <main className="container">{children}</main>
      </body>
    </html>
  );
}
