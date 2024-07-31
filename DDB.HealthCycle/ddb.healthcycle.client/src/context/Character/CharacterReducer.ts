import { PlayerCharacter } from "../../components/characterSheet/types";
import { CharacterActions } from "./actions"
import { CharacterStateType } from "./CharacterState";

type LocalAction = {
  type: CharacterActions,
  payload: null | PlayerCharacter,
};

const CharacterReducer = (state: CharacterStateType, { type, payload }: LocalAction) : CharacterStateType => {
  switch (type) {
    case CharacterActions.LoadCharacter:
      return {
        ...state,
        character: payload as PlayerCharacter
      }
    default:
      return state;
  }
};

export default CharacterReducer;
