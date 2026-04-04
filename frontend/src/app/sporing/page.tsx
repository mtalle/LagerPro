'use client';
import { useEffect, useState } from 'react';
import { get } from '../../lib/api';

// Types
interface SporbarhetTransaksjon {
  id: number;
  type: string;
  mengde: number;
  beholdningEtter: number;
  kilde: string;
  kildeId: number | null;
  kommentar: string | null;
  utfortAv: string | null;
  tidspunkt: string;
}

interface SporbarhetData {
  lotNr: string;
  artikkelId: number;
  artikkelNavn: string | null;
  mengde: number;
  enhet: string;
  transaksjoner: SporbarhetTransaksjon[];
}

interface Kunde {
  id: number;
  navn: string;
  aktiv: boolean;
}

interface Produksjon {
  id: number;
  ordreNr: string;
  reseptNavn: string;
}

type SearchType = 'lot' | 'kunde' | 'batch';

export default function SporingPage() {
  const [searchInput, setSearchInput] = useState('');
  const [searchType, setSearchType] = useState<SearchType>('lot');
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<SporbarhetData | null>(null);
  const [error, setError] = useState('');
  const [kunder, setKunder] = useState<Kunde[]>([]);
  const [produksjoner, setProduksjoner] = useState<Produksjon[]>([]);
  const [kundeNavnMap, setKundeNavnMap] = useState<Record<number, string>>({});

  // Last data
  useEffect(() => {
    async function loadData() {
      try {
        const [kunderData, prodData] = await Promise.all([
          get<Kunde[]>('/kunder'),
          get<Produksjon[]>('/produksjon')
        ]);
        setKunder(kunderData.filter(k => k.aktiv));
        setProduksjoner(prodData);
      } catch (err) {
        console.error('Feil ved lasting:', err);
      }
    }
    loadData();
  }, []);
  
  // Last kundenavn for leveringer
  useEffect(() => {
    async function loadKundeNavn() {
      if (!data || !data.transaksjoner) return;
      
      const leveringer = data.transaksjoner
        .filter(t => t.type === 'Levering' || t.type === 'LeveringBekreftet')
        .filter(t => t.kildeId !== null);
      
      const newMap: Record<number, string> = {};
      
      for (const tx of leveringer) {
        if (tx.kildeId && !kundeNavnMap[tx.kildeId]) {
          try {
            const navn = await getKundeNavn(tx.kildeId);
            newMap[tx.kildeId] = navn;
          } catch (err) {
            console.error(`Kunne ikke laste kundenavn for levering ${tx.kildeId}:`, err);
            newMap[tx.kildeId] = 'Laster...';
          }
        }
      }
      
      if (Object.keys(newMap).length > 0) {
        setKundeNavnMap(prev => ({ ...prev, ...newMap }));
      }
    }
    
    loadKundeNavn();
  }, [data]);

  async function sok(e: React.FormEvent) {
    e.preventDefault();
    if (!searchInput.trim()) return;
    setLoading(true);
    setError('');
    setData(null);
    
    try {
      if (searchType === 'lot') {
        const result = await get<SporbarhetData>(`/traceability/lot/${encodeURIComponent(searchInput.trim())}`);
        setData(result);
      } else if (searchType === 'kunde') {
        let kundeId: number;
        if (!isNaN(parseInt(searchInput))) {
          kundeId = parseInt(searchInput);
        } else {
          const kunde = kunder.find(k => k.navn.toLowerCase() === searchInput.toLowerCase());
          if (!kunde) {
            setError(`Ingen kunde funnet med "${searchInput}".`);
            return;
          }
          kundeId = kunde.id;
        }
        const result = await get<SporbarhetData>(`/traceability/kunde/${kundeId}`);
        setData(result);
      } else if (searchType === 'batch') {
        let batchId: string;
        if (!isNaN(parseInt(searchInput))) {
          const prod = produksjoner.find(p => p.id === parseInt(searchInput));
          if (!prod) {
            setError(`Ingen produksjon funnet med ID "${searchInput}".`);
            return;
          }
          batchId = prod.ordreNr;
        } else {
          const prod = produksjoner.find(p => p.ordreNr.toLowerCase() === searchInput.toLowerCase());
          if (!prod) {
            setError(`Ingen produksjon funnet med "${searchInput}".`);
            return;
          }
          batchId = prod.ordreNr;
        }
        const result = await get<SporbarhetData>(`/traceability/batch/${encodeURIComponent(batchId)}`);
        setData(result);
      }
    } catch (err) {
      setError(`Ingen ${searchType} funnet med "${searchInput}".`);
    } finally {
      setLoading(false);
    }
  }

  function formatDato(dato: string): string {
    try {
      return new Date(dato).toLocaleString('nb-NO', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
      });
    } catch {
      return dato;
    }
  }

  function formatNummer(tall: number): string {
    return Number(tall).toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  // Hjelpefunksjoner for visning
  function getOrdrenummer(produksjonsId: number): string {
    if (!produksjonsId) return 'Ukjent produksjon';
    const prod = produksjoner.find(p => p.id === produksjonsId);
    return prod?.ordreNr || `Produksjon #${produksjonsId}`;
  }
  
  // Funksjon for å hente kundenavn fra levering
  async function getKundeNavn(leveringsId: number): Promise<string> {
    if (!leveringsId) return 'Ukjent kunde';
    try {
      const response = await fetch(`http://167.99.195.94:5000/api/levering/${leveringsId}`);
      const levering = await response.json();
      return levering.kundeNavn || 'Ukjent kunde';
    } catch (err) {
      console.error('Kunne ikke hente kundenavn:', err);
      return 'Feil ved lasting';
    }
  }

  function getUnikeLeveringer() {
    if (!data || !data.transaksjoner) return [];
    const leveringer = data.transaksjoner
      .filter(t => t.type === 'Levering' || t.type === 'LeveringBekreftet')
      .reduce((acc, tx) => {
        const key = tx.kildeId;
        if (key !== null) {
          if (!acc[key] || new Date(tx.tidspunkt) > new Date(acc[key].tidspunkt)) {
            acc[key] = tx;
          }
        }
        return acc;
      }, {} as Record<number, SporbarhetTransaksjon>);
    return Object.values(leveringer);
  }

  function getUnikeTransaksjoner() {
    if (!data || !data.transaksjoner) return [];
    const transaksjoner = data.transaksjoner
      .reduce((acc, tx) => {
        if (tx.type === 'Levering' || tx.type === 'LeveringBekreftet') {
          const key = tx.kildeId !== null ? `leverings-${tx.kildeId}` : `leverings-null-${tx.id}`;
          if (!acc[key] || new Date(tx.tidspunkt) > new Date(acc[key].tidspunkt)) {
            acc[key] = tx;
          }
        } else {
          acc[`${tx.type}-${tx.id}`] = tx;
        }
        return acc;
      }, {} as Record<string, SporbarhetTransaksjon>);
    return Object.values(transaksjoner)
      .sort((a, b) => new Date(b.tidspunkt).getTime() - new Date(a.tidspunkt).getTime());
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>🧭 Sporing</h1>
        <p className="page-subtitle">Spor varer fra vareinntak til vareutlevering</p>
      </div>

      <div className="search-section">
        <form onSubmit={sok} className="search-form">
          <div className="mb-3">
            <div className="btn-group" role="group">
              <button type="button" className={`btn ${searchType === 'lot' ? 'btn-primary' : 'btn-outline-primary'}`} onClick={() => setSearchType('lot')}>🔍 Lot</button>
              <button type="button" className={`btn ${searchType === 'kunde' ? 'btn-primary' : 'btn-outline-primary'}`} onClick={() => setSearchType('kunde')}>👥 Kunde</button>
              <button type="button" className={`btn ${searchType === 'batch' ? 'btn-primary' : 'btn-outline-primary'}`} onClick={() => setSearchType('batch')}>🏭 Produksjon</button>
            </div>
          </div>
          
          <div className="search-row">
            <input type="text" className="search-input" placeholder="Skriv inn søkeord..." value={searchInput} onChange={e => setSearchInput(e.target.value)} autoFocus />
            <button type="submit" className="btn btn-primary" disabled={loading || !searchInput.trim()}>{loading ? 'Søker...' : '🔍 Spor'}</button>
          </div>
        </form>
      </div>

      {error && <div className="error-message">⚠️ {error}</div>}

      {data && (
        <div className="traceability-result">
          <div className="trace-card">
            <div className="trace-card-header">
              <h2>Lot: {data.lotNr}</h2>
              <span className="badge badge-aktiv">Aktiv</span>
            </div>
            <div className="trace-info-grid">
              <div className="trace-info-item">
                <span className="trace-label">Artikkel</span>
                <span className="trace-value">{data.artikkelNavn || 'Ukjent'}</span>
              </div>
              <div className="trace-info-item">
                <span className="trace-label">Beholdning</span>
                <span className="trace-value trace-highlight">{formatNummer(data.mengde)} {data.enhet}</span>
              </div>
            </div>
          </div>

          {/* Produksjonsinformasjon - FIKSET: Viser nå ordrenummer */}
          {data.transaksjoner && data.transaksjoner.some(t => t.type === 'ProduksjonInn' || t.type === 'ProduksjonUt') && (
            <div className="trace-card">
              <div className="trace-card-header"><h3>🏭 Produksjonsinformasjon</h3></div>
              <div className="table-responsive">
                <table className="table table-sm">
                  <thead><tr><th>Produksjonsnummer</th><th>Tidspunkt</th><th>Mengde</th></tr></thead>
                  <tbody>
                    {data.transaksjoner.filter(t => t.type === 'ProduksjonInn' || t.type === 'ProduksjonUt').map((tx, idx) => (
                      <tr key={idx}>
                        <td>
                          {tx.kildeId ? (
                            <button className="btn btn-sm btn-outline-primary" onClick={() => {
                              const ordrenummer = getOrdrenummer(tx.kildeId!);
                              setSearchType('batch');
                              setSearchInput(ordrenummer.includes('#') ? tx.kildeId!.toString() : ordrenummer);
                              setTimeout(() => document.querySelector('form')?.requestSubmit(), 100);
                            }}>
                              {getOrdrenummer(tx.kildeId!)}
                            </button>
                          ) : (
                            <span>Ukjent produksjon</span>
                          )}
                        </td>
                        <td>{formatDato(tx.tidspunkt)}</td>
                        <td>{formatNummer(tx.mengde)} {data.enhet}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {/* Leveringsinformasjon - FIKSET: En linje per levering, viser status */}
          {getUnikeLeveringer().length > 0 && (
            <div className="trace-card">
              <div className="trace-card-header"><h3>📤 Levert til kunder</h3></div>
              <div className="table-responsive">
                <table className="table table-sm">
                  <thead><tr><th>Leveringsnummer</th><th>Kunde</th><th>Tidspunkt</th><th>Mengde</th><th>Status</th></tr></thead>
                  <tbody>
                    {getUnikeLeveringer().map((tx, idx) => (
                      <tr key={idx}>
                        <td>
                          <button className="btn btn-sm btn-outline-info" onClick={() => {
                            setSearchType('lot');
                            setSearchInput(data.lotNr);
                            setTimeout(() => document.querySelector('form')?.requestSubmit(), 100);
                          }}>
                            {tx.kilde} #{tx.kildeId}
                          </button>
                        </td>
                        <td>
                          {tx.kildeId ? (
                            kundeNavnMap[tx.kildeId] || (
                              <span className="text-muted">Laster...</span>
                            )
                          ) : (
                            <span className="text-muted">Ukjent kunde</span>
                          )}
                        </td>
                        <td>{formatDato(tx.tidspunkt)}</td>
                        <td>{formatNummer(tx.mengde)} {data.enhet}</td>
                        <td>
                          <span className={`badge ${tx.type === 'LeveringBekreftet' ? 'bg-success' : 'bg-warning'}`}>
                            {tx.type === 'LeveringBekreftet' ? 'Bekreftet' : 'Planlagt'}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {/* Transaksjoner - FIKSET: En linje per transaksjon */}
          <div className="trace-card">
            <div className="trace-card-header"><h3>📋 Transaksjoner</h3></div>
            {getUnikeTransaksjoner().length === 0 ? (
              <p className="empty-state">Ingen transaksjoner funnet.</p>
            ) : (
              <div className="transaksjon-liste">
                {getUnikeTransaksjoner().map((tx, idx) => (
                  <div key={idx} className="transaksjon-rad">
                    <div className="transaksjon-type-badge">
                      <span>{tx.type === 'Levering' ? '📤 Levering' : tx.type === 'LeveringBekreftet' ? '✅ Levering bekreftet' : tx.type === 'ProduksjonInn' ? '🏭 Produksjon (INN)' : tx.type === 'ProduksjonUt' ? '🏭 Produksjon (UT)' : tx.type === 'Mottak' ? '📥 Mottak' : '⚙️ Justering'}</span>
                    </div>
                    <div className="transaksjon-detaljer">
                      <div className="transaksjon-hoved">
                        <span className="tx-mengde">{formatNummer(tx.mengde)} {data.enhet}</span>
                        <span className="txBeholdning">→ Beholdning: {formatNummer(tx.beholdningEtter)}</span>
                      </div>
                      <div className="transaksjon-meta">
                        <span>📅 {formatDato(tx.tidspunkt)}</span>
                        {tx.kilde && <span>📂 {tx.kilde} {tx.kildeId ? `#${tx.kildeId}` : ''}</span>}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      <style jsx>{`
        .page-container { padding: 24px; max-width: 1000px; margin: 0 auto; }
        .page-header { margin-bottom: 24px; }
        .page-header h1 { font-size: 28px; margin-bottom: 4px; }
        .page-subtitle { color: #64748b; font-size: 14px; }
        .search-section { background: white; border-radius: 12px; padding: 20px; margin-bottom: 24px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .search-row { display: flex; gap: 12px; }
        .search-input { flex: 1; padding: 12px 16px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 16px; }
        .error-message { background: #fef2f2; border: 1px solid #fecaca; color: #dc2626; padding: 12px 16px; border-radius: 8px; margin-bottom: 24px; }
        .traceability-result { display: flex; flex-direction: column; gap: 20px; }
        .trace-card { background: white; border-radius: 12px; padding: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .trace-card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 1px solid #e2e8f0; }
        .trace-info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; }
        .trace-info-item { display: flex; flex-direction: column; gap: 2px; }
        .trace-label { font-size: 12px; color: #64748b; text-transform: uppercase; }
        .trace-value { font-size: 16px; font-weight: 500; }
        .trace-highlight { font-size: 20px; color: #3b82f6; font-weight: 600; }
        .transaksjon-liste { display: flex; flex-direction: column; gap: 12px; }
        .transaksjon-rad { display: flex; gap: 16px; padding: 12px; border-radius: 8px; border-left: 4px solid #cbd5e1; background: #f8fafc; }
        .transaksjon-type-badge { min-width: 140px; }
        .transaksjon-type-badge span { font-size: 13px; font-weight: 500; }
        .transaksjon-detaljer { flex: 1; }
        .transaksjon-hoved { display: flex; gap: 16px; align-items: baseline; margin-bottom: 4px; }
        .tx-mengde { font-weight: 600; font-size: 15px; }
        .txBeholdning { color: #64748b; font-size: 13px; }
        .transaksjon-meta { display: flex; gap: 16px; font-size: 12px; color: #64748b; flex-wrap: wrap; }
        .empty-state { text-align: center; padding: 24px; color: #64748b; }
        .badge { padding: 4px 10px; border-radius: 20px; font-size: 12px; font-weight: 500; }
        .badge-aktiv { background: #dcfce7; color: #16a34a; }
      `}</style>
    </div>
  );
}