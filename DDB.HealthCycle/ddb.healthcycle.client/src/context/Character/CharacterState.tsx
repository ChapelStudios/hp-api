import { useReducer } from "react";
import { PlayerCharacter } from "../../components/characterSheet/types"
import CharacterContext from "./CharacterContext";
import CharacterReducer from "./CharacterReducer";
import RestService from "../../services/restService";
import { CharacterApiPaths } from "../../constants/ApiPaths";
import { CharacterActions } from "./actions";

export type CharacterStateType = {
  character: PlayerCharacter;
};

export const CharacterStateInitialState: CharacterStateType = Object.freeze({
  character: {} as PlayerCharacter,
});

const CharacterState = ({ children }: { children: React.ReactNode }) => {
  const [state, dispatch] = useReducer(CharacterReducer, CharacterStateInitialState);

  const loadCharacter = async (id: string) => {
    return await RestService.get<PlayerCharacter>({
      uri: CharacterApiPaths.getCharacter(id),
      onSuccess: (result) => {
        dispatch({
          type: CharacterActions.LoadCharacter,
          payload: result,
        })
      },
    });
  }

  return (
    <CharacterContext.Provider value={{
      character: state.character,
      loadCharacter,
    }} >
      {children}
    </CharacterContext.Provider>
  )
};

export default CharacterState;
