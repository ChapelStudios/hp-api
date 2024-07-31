import { createContext } from "react";
import { PlayerCharacter } from "../../components/characterSheet/types";

export type CharacterContextValue = {
  character: PlayerCharacter;
  loadCharacter: (id: string) => Promise<boolean>;
};

const CharacterContext = createContext<CharacterContextValue>(undefined!);

export default CharacterContext;
