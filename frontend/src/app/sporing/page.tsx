'use client';
import { useEffect, useState } from 'react';
import { get } from '../../lib/api';

// Types matching API contracts
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
  artikkelNr: string | null;
  artikkelNavn: string | null;
  mengde: number;
  enhet: string;
  bestForDato: string | null;
  sistOppdatert: string;
  lokasjon: string | null;
  transaksjoner: SporbarhetTransaksjon[];
}

interface Kunde {
  id: number;
  navn: string;
  kontaktperson?: string;
  telefon?: string;
  epost?: string;
  adresse?: string;
  postnr?: string;
  poststed?: string;
  orgNr?: string;
  kommentar?: string;
  aktiv: boolean;
  opprettetDato: string;
}

interface Produksjon {
  id: number;
  reseptId: number;
  reseptNavn: string;
  ordreNr: string;
  planlagtDato: string;
  ferdigmeldtDato: string | null;
  antallProdusert: number;
  ferdigvareLotNr: string;
  status: string;
  kommentar: string | null;
  utfortAv: string | null;
  opprettetDato: string;
  ferdigvareId: number;
  ferdigvareNavn: string;
  ferdigvareEnhet: string;
}

type SearchType = 'lot' | 'kunde' | 'batch';

function transaksjonTypeLabel(type: string): string {
  switch (type) {
    case 'Mottak': return '📥 Mottak';
    case 'Levering': return '📤 Levering';
    case 'ProduksjonInn': return '🏭 Produksjon (INN)';
    case 'ProduksjonUt': return '🏭 Produksjon (UT)';
    case 'Justering': return '⚙️ Justering';
    case 'LeveringBekreftet': return '✅ Levering bekreftet';
    default: return type;
  }
}

function transaksjonFarge(type: string): string {
  switch (type) {
    case 'Mottak': return 'status-mottatt';
    case 'Levering': return 'status-levert';
    case 'ProduksjonInn': return 'status-produksjon';
    case 'ProduksjonUt': return 'status-planlagt';
    case 'Justering': return 'status-aktiv';
    case 'LeveringBekreftet': return 'status-godkjent';
    default: return '';
  }
}

export default function SporingPage() {
  const [searchInput, setSearchInput] = useState('');
  const [searchType, setSearchType] = useState<SearchType>('lot');
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<SporbarhetData | null>(null);
  const [error, setError] = useState('');
  const [kunder, setKunder] = useState<Kunde[]>([]);
  const [produksjoner, setProduksjoner] = useState<Produksjon[]>([]);
  const [showKundeSuggestions, setShowKundeSuggestions] = useState(false);
  const [showProduksjonSuggestions, setShowProduksjonSuggestions] = useState(false);
  const [filteredKunder, setFilteredKunder] = useState<Kunde[]>([]);
  const [filteredProduksjoner, setFilteredProduksjoner] = useState<Produksjon[]>([]);

  // Hent kundeliste og produksjonsliste ved første lasting
  useEffect(() => {
    async function loadKunder() {
      try {
        const data = await get<Kunde[]>('/kunder');
        setKunder(data.filter(k => k.aktiv)); // Bare aktive kunder
      } catch (err) {
        console.error('Kunne ikke laste kunder:', err);
      }
    }
    
    async function loadProduksjoner() {
      try {
        const data = await get<Produksjon[]>('/produksjon');
        setProduksjoner(data);
      } catch (err) {
        console.error('Kunne ikke laste produksjoner:', err);
      }
    }
    
    loadKunder();
    loadProduksjoner();
  }, []);

  // Filtrer kunder basert på søk
  useEffect(() => {
    if (searchType === 'kunde' && searchInput.trim()) {
      const query = searchInput.toLowerCase();
      const filtered = kunder.filter(k => 
        k.navn.toLowerCase().includes(query) || 
        (k.orgNr && k.orgNr.includes(query)) ||
        (k.id.toString() === query)
      );
      setFilteredKunder(filtered.slice(0, 5)); // Vis maks 5 forslag
      setShowKundeSuggestions(filtered.length > 0);
    } else {
      setShowKundeSuggestions(false);
    }
  }, [searchInput, searchType, kunder]);

  // Filtrer produksjoner basert på søk
  useEffect(() => {
    if (searchType === 'batch' && searchInput.trim()) {
      const query = searchInput.toLowerCase();
      const filtered = produksjoner.filter(p => 
        p.ordreNr.toLowerCase().includes(query) || 
        p.reseptNavn.toLowerCase().includes(query) ||
        p.ferdigvareLotNr.toLowerCase().includes(query) ||
        (p.id.toString() === query)
      );
      setFilteredProduksjoner(filtered.slice(0, 5)); // Vis maks 5 forslag
      setShowProduksjonSuggestions(filtered.length > 0);
    } else {
      setShowProduksjonSuggestions(false);
    }
  }, [searchInput, searchType, produksjoner]);

  async function sok(e: React.FormEvent) {
    e.preventDefault();
    if (!searchInput.trim()) return;
    setLoading(true);
    setError('');
    setData(null);
    setShowKundeSuggestions(false);
    setShowProduksjonSuggestions(false);
    
    try {
      if (searchType === 'lot') {
        const result = await get<SporbarhetData>(`/traceability/lot/${encodeURIComponent(searchInput.trim())}`);
        setData(result);
      } else if (searchType === 'kunde') {
        // Søk kan være kunde-ID, navn eller org.nr
        let kundeId: number;
        
        // Sjekk om input er et tall (kunde-ID)
        if (!isNaN(parseInt(searchInput))) {
          kundeId = parseInt(searchInput);
        } else {
          // Søk på navn eller org.nr
          const kunde = kunder.find(k => 
            k.navn.toLowerCase() === searchInput.toLowerCase() ||
            k.orgNr === searchInput
          );
          
          if (!kunde) {
            setError(`Ingen kunde funnet med "${searchInput}". Prøv kunde-ID, navn eller org.nr.`);
            return;
          }
          kundeId = kunde.id;
        }
        
        // Bruk traceability/kunde API
        const result = await get<SporbarhetData>(`/traceability/kunde/${kundeId}`);
        setData(result);
      } else if (searchType === 'batch') {
        // Søk kan være produksjons-ID, ordreNr, reseptnavn eller ferdigvareLotNr
        let batchId: string;
        
        // Sjekk om input er et tall (produksjons-ID)
        if (!isNaN(parseInt(searchInput))) {
          const prod = produksjoner.find(p => p.id === parseInt(searchInput));
          if (!prod) {
            setError(`Ingen produksjon funnet med ID "${searchInput}".`);
            return;
          }
          batchId = prod.ordreNr;
        } else {
          // Søk på ordreNr, reseptNavn eller ferdigvareLotNr
          const prod = produksjoner.find(p => 
            p.ordreNr.toLowerCase() === searchInput.toLowerCase() ||
            p.reseptNavn.toLowerCase() === searchInput.toLowerCase() ||
            p.ferdigvareLotNr.toLowerCase() === searchInput.toLowerCase()
          );
          
          if (!prod) {
            setError(`Ingen produksjon funnet med "${searchInput}". Prøv ordrenummer, reseptnavn eller ferdigvare-lot.`);
            return;
          }
          batchId = prod.ordreNr;
        }
        
        // Bruk traceability/batch API
        const result = await get<SporbarhetData>(`/traceability/batch/${encodeURIComponent(batchId)}`);
        setData(result);
      }
    } catch (err) {
      const msg = (err as Error).message || '';
      if (msg.includes('404')) {
        setError(`Ingen ${searchType === 'lot' ? 'lot' : searchType === 'kunde' ? 'kunde' : 'batch'} funnet med "${searchInput}".`);
      } else {
        setError('Feil ved lasting: ' + msg);
      }
    } finally {
      setLoading(false);
    }
  }

  function handleLotClick(lotNr: string) {
    setSearchType('lot');
    setSearchInput(lotNr);
    setTimeout(() => {
      const form = document.querySelector('form');
      if (form) form.requestSubmit();
    }, 100);
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

  function formatDatoKort(dato: string): string {
    try {
      return new Date(dato).toLocaleDateString('nb-NO');
    } catch {
      return dato;
    }
  }

  function formatNummer(tall: number): string {
    return Number(tall).toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>🧭 Sporing</h1>
        <p className="page-subtitle">Spor varer fra vareinntak til vareutlevering via lot-nummer, kunde eller produksjon</p>
      </div>

      <div className="search-section">
        <form onSubmit={sok} className="search-form">
          <div className="mb-3">
            <div className="btn-group" role="group">
              <button 
                type="button" 
                className={`btn ${searchType === 'lot' ? 'btn-primary' : 'btn-outline-primary'}`}
                onClick={() => setSearchType('lot')}
              >
                🔍 Lot-nummer
              </button>
              <button 
                type="button" 
                className={`btn ${searchType === 'kunde' ? 'btn-primary' : 'btn-outline-primary'}`}
                onClick={() => setSearchType('kunde')}
              >
                👥 Kunde
              </button>
              <button 
                type="button" 
                className={`btn ${searchType === 'batch' ? 'btn-primary' : 'btn-outline-primary'}`}
                onClick={() => setSearchType('batch')}
              >
                🏭 Produksjon
              </button>
            </div>
          </div>
          
          <div className="search-row position-relative">
            <input
              type="text"
              className="search-input"
              placeholder={searchType === 'lot' ? 'Skriv inn lot-nummer, f.eks. LOT-2024-001...' : 
                         searchType === 'kunde' ? 'Skriv kunde-ID, navn eller org.nr...' : 
                         'Skriv inn produksjonsnummer...'}
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
              onFocus={() => {
                if (searchType === 'kunde') setShowKundeSuggestions(true);
                if (searchType === 'batch') setShowProduksjonSuggestions(true);
              }}
              autoFocus
            />
            <button type="submit" className="btn btn-primary" disabled={loading || !searchInput.trim()}>
              {loading ? 'Søker...' : '🔍 Spor'}
            </button>
            
            {/* Kunde-forslag */}
            {showKundeSuggestions && filteredKunder.length > 0 && (
              <div className="suggestions-dropdown">
                {filteredKunder.map(kunde => (
                  <div 
                    key={kunde.id} 
                    className="suggestion-item"
                    onClick={() => {
                      setSearchInput(kunde.navn);
                      setShowKundeSuggestions(false);
                    }}
                  >
                    <div className="fw-medium">{kunde.navn}</div>
                    <div className="small text-muted">
                      ID: {kunde.id} • {kunde.orgNr ? `Org.nr: ${kunde.orgNr}` : 'Ingen org.nr'}
                    </div>
                  </div>
                ))}
              </div>
            )}
            
            {/* Produksjons-forslag */}
            {showProduksjonSuggestions && filteredProduksjoner.length > 0 && (
              <div className="suggestions-dropdown">
                {filteredProduksjoner.map(prod => (
                  <div 
                    key={prod.id} 
                    className="suggestion-item"
                    onClick={() => {
                      setSearchInput(prod.ordreNr);
                      setShowProduksjonSuggestions(false);
                    }}
                  >
                    <div className="fw-medium">{prod.ordreNr}</div>
                    <div className="small text-muted">
                      {prod.reseptNavn} • {prod.status} • Ferdigvare: {prod.ferdigvareLotNr}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
          
          <div className="mt-2 text-muted small">
            {searchType === 'lot' && 'Søk på lot-nummer for å se full historikk, produksjoner og leveringer.'}
            {searchType === 'kunde' && 'Søk på kunde-ID, navn eller org.nr for å se alle leveringer til kunden.'}
            {searchType === 'batch' && 'Søk på produksjonsnummer for å se forbrukte råvarer og ferdigvarer.'}
          </div>
        </form>
      </div>

      {error && (
        <div className="error-message">
          <span className="error-icon">⚠️</span> {error}
        </div>
      )}

      {data && (
        <div className="traceability-result">
          {/* Hovedinfo */}
          <div className="trace-card">
            <div className="trace-card-header">
              <h2>Lot: {data.lotNr}</h2>
              <span className="badge badge-aktiv">Aktiv</span>
            </div>
            <div className="trace-info-grid">
              <div className="trace-info-item">
                <span className="trace-label">Artikkel</span>
                <span className="trace-value">{data.artikkelNavn || 'Ukjent'}</span>
                <span className="trace-sub">{data.artikkelNr}</span>
              </div>
              <div className="trace-info-item">
                <span className="trace-label">Beholdning</span>
                <span className="trace-value trace-highlight">{formatNummer(data.mengde)} {data.enhet}</span>
              </div>
              <div className="trace-info-item">
                <span className="trace-label">Lokasjon</span>
                <span className="trace-value">{data.lokasjon || 'Ikke angitt'}</span>
              </div>
              <div className="trace-info-item">
                <span className="trace-label">Best før</span>
                <span className="trace-value">{data.bestForDato ? formatDatoKort(data.bestForDato) : 'Ikke angitt'}</span>
              </div>
              <div className="trace-info-item">
                <span className="trace-label">Sist oppdatert</span>
                <span className="trace-value">{formatDato(data.sistOppdatert)}</span>
              </div>
            </div>
          </div>

          {/* Produksjonsinformasjon */}
          {data.transaksjoner.some(t => t.type === 'ProduksjonInn' || t.type === 'ProduksjonUt') && (
            <div className="trace-card">
              <div className="trace-card-header">
                <h3>🏭 Produksjonsinformasjon</h3>
              </div>
              
              {data.transaksjoner.filter(t => t.type === 'ProduksjonInn').length > 0 && (
                <div className="mb-4">
                  <h4 className="mb-2">Gått inn i produksjon:</h4>
                  <div className="table-responsive">
                    <table className="table table-sm table-hover">
                      <thead>
                        <tr>
                          <th>Produksjonsnummer</th>
                          <th>Tidspunkt</th>
                          <th>Mengde</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.transaksjoner
                          .filter(t => t.type === 'ProduksjonInn')
                          .map((tx, idx) => (
                            <tr key={idx}>
                              <td>
                                <button 
                                  className="btn btn-sm btn-outline-primary"
                                  onClick={() => {
                                    setSearchType('batch');
                                    setSearchInput(tx.kildeId?.toString() || '');
                                    setTimeout(() => {
                                      const form = document.querySelector('form');
                                      if (form) form.requestSubmit();
                                    }, 100);
                                  }}
                                >
                                  {tx.kilde} #{tx.kildeId}
                                </button>
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
              
              {data.transaksjoner.filter(t => t.type === 'ProduksjonUt').length > 0 && (
                <div>
                  <h4 className="mb-2">Produsert fra:</h4>
                  <div className="table-responsive">
                    <table className="table table-sm table-hover">
                      <thead>
                        <tr>
                          <th>Produksjonsnummer</th>
                          <th>Tidspunkt</th>
                          <th>Mengde</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.transaksjoner
                          .filter(t => t.type === 'ProduksjonUt')
                          .map((tx, idx) => (
                            <tr key={idx}>
                              <td>
                                <button 
                                  className="btn btn-sm btn-outline-primary"
                                  onClick={() => {
                                    setSearchType('batch');
                                    setSearchInput(tx.kildeId?.toString() || '');
                                    setTimeout(() => {
                                      const form = document.querySelector('form');
                                      if (form) form.requestSubmit();
                                    }, 100);
                                  }}
                                >
                                  {tx.kilde} #{tx.kildeId}
                                </button>
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
            </div>
          )}

          {/* Leveringsinformasjon */}
          {data.transaksjoner.some(t => t.type === 'Levering' || t.type === 'LeveringBekreftet') && (
            <div className="trace-card">
              <div className="trace-card-header">
                <h3>📤 Levert til kunder</h3>
              </div>
              <div className="table-responsive">
                <table className="table table-sm table-hover">
                  <thead>
                    <tr>
                      <th>Leveringsnummer</th>
                      <th>Tidspunkt</th>
                      <th>Mengde</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data.transaksjoner
                      .filter(t => t.type === 'Levering' || t.type === 'LeveringBekreftet')
                      .map((tx, idx) => (
                        <tr key={idx}>
                          <td>
                            <button 
                              className="btn btn-sm btn-outline-info"
                              onClick={() => {
                                // Her må vi hente kunde-ID fra leveringen
                                // For nå: søk på leveringsnummer
                                setSearchType('lot');
                                setSearchInput(data.lotNr);
                                setTimeout(() => {
                                  const form = document.querySelector('form');
                                  if (form) form.requestSubmit();
                                }, 100);
                              }}
                            >
                              {tx.kilde} #{tx.kildeId}
                            </button>
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

          {/* Transaksjoner */}
          <div className="trace-card">
            <div className="trace-card-header">
              <h3>📋 Transaksjoner ({data.transaksjoner.length})</h3>
            </div>

            {data.transaksjoner.length === 0 ? (
              <p className="empty-state">Ingen transaksjoner funnet for denne lotten.</p>
            ) : (
              <div className="transaksjon-liste">
                {data.transaksjoner
                  .sort((a, b) => new Date(b.tidspunkt).getTime() - new Date(a.tidspunkt).getTime())
                  .map((tx, idx) => (
                    <div key={tx.id} className={`transaksjon-rad ${transaksjonFarge(tx.type)}`}>
                      <div className="transaksjon-type-badge">
                        <span>{transaksjonTypeLabel(tx.type)}</span>
                      </div>
                      <div className="transaksjon-detaljer">
                        <div className="transaksjon-hoved">
                          <span className="tx-mengde">{formatNummer(tx.mengde)} {data.enhet}</span>
                          <span className="txBeholdning">→ Beholdning: {formatNummer(tx.beholdningEtter)}</span>
                        </div>
                        <div className="transaksjon-meta">
                          <span>📅 {formatDato(tx.tidspunkt)}</span>
                          {tx.kilde && <span>📂 {tx.kilde} {tx.kildeId ? `#${tx.kildeId}` : ''}</span>}
                          {tx.utfortAv && <span>👤 {tx.utfortAv}</span>}
                        </div>
                        {tx.kommentar && (
                          <div className="transaksjon-kommentar">💬 {tx.kommentar}</div>
                        )}
                      </div>
                    </div>
                  ))}
              </div>
            )}
          </div>
        </div>
      )}

      {!data && !error && !loading && (
        <div className="empty-state-large">
          <div className="empty-icon">🔍</div>
          <h3>Velg søketype og skriv inn søkeord</h3>
          <p>Bruk knappene over for å velge om du vil søke på lot-nummer, kunde eller produksjon.</p>
          <p className="empty-hint">Tips: Klikk på lot-nummer i lagersiden for å gå direkte til sporing.</p>
        </div>
      )}

      <style jsx>{`
        .page-container { padding: 24px; max-width: 1000px; margin: 0 auto; }
        .page-header { margin-bottom: 24px; }
        .page-header h1 { font-size: 28px; margin-bottom: 4px; }
        .page-subtitle { color: #64748b; font-size: 14px; }
        .search-section { background: white; border-radius: 12px; padding: 20px; margin-bottom: 24px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .search-form { }
        .search-row { display: flex; gap: 12px; }
        .search-input { flex: 1; padding: 12px 16px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 16px; }
        .search-input:focus { outline: none; border-color: #3b82f6; }
        .error-message { background: #fef2f2; border: 1px solid #fecaca; color: #dc2626; padding: 12px 16px; border-radius: 8px; margin-bottom: 24px; display: flex; align-items: center; gap: 8px; }
        .traceability-result { display: flex; flex-direction: column; gap: 20px; }
        .trace-card { background: white; border-radius: 12px; padding: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .trace-card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 1px solid #e2e8f0; }
        .trace-card-header h2 { font-size: 20px; margin: 0; }
        .trace-card-header h3 { font-size: 16px; margin: 0; }
        .trace-info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; }
        .trace-info-item { display: flex; flex-direction: column; gap: 2px; }
        .trace-label { font-size: 12px; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; }
        .trace-value { font-size: 16px; font-weight: 500; }
        .trace-highlight { font-size: 20px; color: #3b82f6; font-weight: 600; }
        .trace-sub { font-size: 13px; color: #94a3b8; }
        .transaksjon-liste { display: flex; flex-direction: column; gap: 12px; }
        .transaksjon-rad { display: flex; gap: 16px; padding: 12px; border-radius: 8px; border-left: 4px solid #cbd5e1; background: #f8fafc; }
        .transaksjon-rad.status-mottatt { border-left-color: #22c55e; background: #f0fdf4; }
        .transaksjon-rad.status-levert { border-left-color: #ef4444; background: #fef2f2; }
        .transaksjon-rad.status-produksjon { border-left-color: #8b5cf6; background: #faf5ff; }
        .transaksjon-rad.status-planlagt { border-left-color: #f59e0b; background: #fffbeb; }
        .transaksjon-rad.status-godkjent { border-left-color: #10b981; background: #ecfdf5; }
        .transaksjon-rad.status-aktiv { border-left-color: #3b82f6; background: #eff6ff; }
        .transaksjon-type-badge { min-width: 140px; }
        .transaksjon-type-badge span { font-size: 13px; font-weight: 500; }
        .transaksjon-detaljer { flex: 1; }
        .transaksjon-hoved { display: flex; gap: 16px; align-items: baseline; margin-bottom: 4px; }
        .tx-mengde { font-weight: 600; font-size: 15px; }
        .txBeholdning { color: #64748b; font-size: 13px; }
        .transaksjon-meta { display: flex; gap: 16px; font-size: 12px; color: #64748b; flex-wrap: wrap; }
        .transaksjon-meta span { display: flex; align-items: center; gap: 4px; }
        .transaksjon-kommentar { margin-top: 6px; font-size: 13px; color: #475569; font-style: italic; }
        .empty-state { text-align: center; padding: 24px; color: #64748b; }
        .empty-state-large { text-align: center; padding: 60px 24px; }
        .empty-icon { font-size: 48px; margin-bottom: 16px; }
        .empty-state-large h3 { font-size: 20px; margin-bottom: 8px; color: #334155; }
        .empty-state-large p { color: #64748b; margin-bottom: 8px; }
        .empty-hint { font-size: 13px; }
        .badge { padding: 4px 10px; border-radius: 20px; font-size: 12px; font-weight: 500; }
        .badge-aktiv { background: #dcfce7; color: #16a34a; }
        .suggestions-dropdown { 
          position: absolute; 
          top: 100%; 
          left: 0; 
          right: 0; 
          background: white; 
          border: 1px solid #e2e8f0; 
          border-radius: 8px; 
          box-shadow: 0 4px 12px rgba(0,0,0,0.1); 
          z-index: 1000; 
          margin-top: 4px; 
          max-height: 300px; 
          overflow-y: auto; 
        }
        .suggestion-item { 
          padding: 12px 16px; 
          cursor: pointer; 
          border-bottom: 1px solid #f1f5f9; 
        }
        .suggestion-item:hover { 
          background: #f8fafc; 
        }
        .suggestion-item:last-child { 
          border-bottom: none; 
        }
      `}</style>
    </div>
  );
}