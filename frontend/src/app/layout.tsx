'use client';
import { useState, useEffect } from 'react';
import './globals.css';

export default function RootLayout({ children }: { children: React.ReactNode }) {
  const [menuOpen, setMenuOpen] = useState(false);
  const [brukerNavn, setBrukerNavn] = useState<string | null>(null);

  useEffect(() => {
    setBrukerNavn(localStorage.getItem('lagerpro_bruker_navn'));
  }, []);

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
            <a href="/" onClick={() => setMenuOpen(false)}>Dashboard</a>
            <a href="/artikler" onClick={() => setMenuOpen(false)}>Artikler</a>
            <a href="/kunder" onClick={() => setMenuOpen(false)}>Kunder</a>
            <a href="/leverandorer" onClick={() => setMenuOpen(false)}>Leverandører</a>
            <a href="/lager" onClick={() => setMenuOpen(false)}>Lager</a>
            <a href="/mottak" onClick={() => setMenuOpen(false)}>Mottak</a>
            <a href="/produksjon" onClick={() => setMenuOpen(false)}>Produksjon</a>
            <a href="/levering" onClick={() => setMenuOpen(false)}>Levering</a>
            <a href="/resepter" onClick={() => setMenuOpen(false)}>Resepter</a>
            <a href="/brukere" onClick={() => setMenuOpen(false)}>👤 Admin</a>
          </div>
          {brukerNavn && (
            <div className="nav-bruker" style={{ marginLeft: 'auto', padding: '0 1rem', fontSize: '0.85rem', color: '#6b7280' }}>
              Innlogget: <strong>{brukerNavn}</strong>
            </div>
          )}
        </nav>
        <main className="container">{children}</main>
      </body>
    </html>
  );
}
