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
  const [lotNr, setLotNr] = useState('');
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<SporbarhetData | null>(null);
  const [error, setError] = useState('');
  const [sokLast, setSokLast] = useState('');

  async function sok(e: React.FormEvent) {
    e.preventDefault();
    if (!lotNr.trim()) return;
    setLoading(true);
    setError('');
    setData(null);
    try {
      const result = await get<SporbarhetData>(`/traceability/lot/${encodeURIComponent(lotNr.trim())}`);
      setData(result);
    } catch (err) {
      const msg = (err as Error).message || '';
      if (msg.includes('404')) {
        setError(`Ingen lot funnet med nummer "${lotNr}".`);
      } else {
        setError('Feil ved lasting: ' + msg);
      }
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

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>🧭 Sporing</h1>
        <p className="page-subtitle">Spor varer fra vareinntak til vareutlevering via lot-nummer</p>
      </div>

      <div className="search-section">
        <form onSubmit={sok} className="search-form">
          <div className="search-row">
            <input
              type="text"
              className="search-input"
              placeholder="Skriv inn lot-nummer, f.eks. LOT-2024-001..."
              value={lotNr}
              onChange={e => setLotNr(e.target.value)}
              autoFocus
            />
            <button type="submit" className="btn btn-primary" disabled={loading || !lotNr.trim()}>
              {loading ? 'Søker...' : '🔍 Spor'}
            </button>
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
                <span className="trace-value">{data.bestForDato ? new Date(data.bestForDato).toLocaleDateString('nb-NO') : 'Ikke angitt'}</span>
              </div>
              <div className="trace-info-item">
                <span className="trace-label">Sist oppdatert</span>
                <span className="trace-value">{formatDato(data.sistOppdatert)}</span>
              </div>
            </div>
          </div>

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
                          {tx.kilde && <span>📂 {tx.kilde}</span>}
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
          <h3>Søk på lot-nummer</h3>
          <p>Skriv inn lot-nummeret du vil spore for å se full historikk.</p>
          <p className="empty-hint">Tips: Lot-nummer finner du på lagersiden eller ved varemottak.</p>
        </div>
      )}

      <style jsx>{`
        .page-container { padding: 24px; max-width: 900px; margin: 0 auto; }
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
      `}</style>
    </div>
  );
}
