import { createContext } from 'react';

interface GameModeContext {
  username: string;
  selectedMode: string;
}

export const GameModeContext = createContext<GameModeContext | null>(null);