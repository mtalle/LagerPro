import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'LagerPro MVP',
  description: 'Lager- og produksjonsstyring',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="no">
      <body>
        <nav className="navbar">
          <span className="logo">📦 LagerPro</span>
          <div className="nav-links">
            <a href="/">Artikler</a>
            <a href="/lager">Lager</a>
            <a href="/mottak">Mottak</a>
            <a href="/produksjon">Produksjon</a>
            <a href="/levering">Levering</a>
          </div>
        </nav>
        <main className="container">{children}</main>
      </body>
    </html>
  );
}
