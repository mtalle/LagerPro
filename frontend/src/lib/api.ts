const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000/api';

export async function get<T>(path: string): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`);
  if (!res.ok) throw new Error(`${path}: ${res.status}`);
  return res.json();
}

export async function post<T>(path: string, body: unknown): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`${path}: ${res.status}`);
  return res.json();
}

export async function put<T>(path: string, body: unknown): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`${path}: ${res.status}`);
  return res.json();
}

export async function patch<T>(path: string, body: unknown): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`${path}: ${res.status}`);
  return res.json();
}

export interface UpdateMottakLinje {
  godkjent: boolean;
  avvik?: string;
}

export async function del(path: string): Promise<void> {
  const res = await fetch(`${API_BASE}${path}`, { method: 'DELETE' });
  if (!res.ok) throw new Error(`${path}: ${res.status}`);
}

// Types matching the API contracts
export interface Article {
  id: number;
  artikkelNr: string;
  navn: string;
  enhet: string;
  type: string;
  beskrivelse?: string;
  strekkode?: string;
  kategori?: string;
  innpris: number;
  utpris: number;
  minBeholdning: number;
  aktiv: boolean;
}

export interface CreateArticle {
  artikkelNr: string;
  navn: string;
  enhet: string;
  type: string;
  beskrivelse?: string;
  strekkode?: string;
  kategori?: string;
  innpris: number;
  utpris: number;
  minBeholdning: number;
}

export interface UpdateArticle extends CreateArticle {
  aktiv: boolean;
}

export interface LagerBeholdning {
  id: number;
  artikkelId: number;
  artikkelNavn: string;
  artikkelNr: string;
  lotNr: string;
  mengde: number;
  enhet: string;
  lokasjon?: string;
  bestForDato?: string;
  sistOppdatert: string;
  minBeholdning?: number;
}

export interface JusterLagerRequest {
  artikkelId: number;
  lotNr: string;
  nyMengde: number;
  kommentar?: string;
  utfortAv?: string;
}

export interface Mottak {
  id: number;
  leverandorId: number;
  leverandorNavn?: string;
  mottaksDato: string;
  referanse?: string;
  kommentar?: string;
  mottattAv?: string;
  status: string;
  opprettetDato: string;
  linjer: MottakLinje[];
}

export interface MottakLinje {
  id: number;
  artikkelId: number;
  artikkelNavn?: string;
  lotNr: string;
  mengde: number;
  enhet: string;
  bestForDato?: string;
  temperatur?: number;
  strekkode?: string;
  avvik?: string;
  kommentar?: string;
  godkjent: boolean;
}

export interface ProduksjonsOrdre {
  id: number;
  reseptId: number;
  reseptNavn?: string;
  ordreNr: string;
  planlagtDato: string;
  ferdigmeldtDato?: string;
  antallProdusert: number;
  ferdigvareLotNr: string;
  status: string;
  kommentar?: string;
  utfortAv?: string;
  opprettetDato: string;
  ferdigvareId?: number;
  ferdigvareNavn?: string;
  ferdigvareEnhet?: string;
}

export interface Levering {
  id: number;
  kundeId: number;
  kundeNavn?: string;
  leveringsDato: string;
  referanse?: string;
  fraktBrev?: string;
  kommentar?: string;
  status: string;
  levertAv?: string;
  opprettetDato: string;
  linjer: LeveringLinje[];
}

export interface LeveringLinje {
  id: number;
  artikkelId: number;
  artikkelNavn: string;
  lotNr: string;
  mengde: number;
  enhet: string;
}

export interface Kunde {
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
}

export interface Leverandor {
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
}

export interface ReseptLinje {
  id: number;
  ravareId: number;
  ravareNavn?: string;
  mengde: number;
  enhet: string;
  rekkefolge: number;
  kommentar?: string;
}

export interface Resept {
  id: number;
  navn: string;
  ferdigvareId: number;
  ferdigvareNavn?: string;
  beskrivelse?: string;
  antallPortjoner: number;
  instruksjoner?: string;
  aktiv: boolean;
  linjer: ReseptLinje[];
}

export interface PlukklisteLinje {
  ordreNr: string;
  reseptId: number;
  reseptNavn: string | null;
  ferdigvareNavn: string;
  planlagtAntall: number;
  feltAntall: number | null;
  ravareId: number;
  ravareNavn: string | null;
  lotNr: string;
  mengde: number;
  enhet: string;
  status: string;
}

export interface Plukkliste {
  linjer: PlukklisteLinje[];
  totaltAntallLinjer: number;
}
