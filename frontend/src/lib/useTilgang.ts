'use client';
import { useState, useEffect } from 'react';
import { Bruker, getMe } from './api';

export function useTilgang(ressursId: number): boolean {
  const [hasAccess, setHasAccess] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function check() {
      try {
        const uid = localStorage.getItem('lagerpro_bruker_id');
        if (!uid) { setLoading(false); return; }
        const b: Bruker = await getMe();
        if (b.erAdmin) { setHasAccess(true); setLoading(false); return; }
        setHasAccess(b.tilganger.some(t => t.ressursId === ressursId));
      } catch { setHasAccess(false); }
      finally { setLoading(false); }
    }
    check();
  }, [ressursId]);

  return hasAccess;
}
