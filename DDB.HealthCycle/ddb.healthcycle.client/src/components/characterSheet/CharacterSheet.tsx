import { useContext, useEffect } from 'react'
import CharacterContext from '../../context/Character/CharacterContext';

const CharacterSheet = () => {
  const {
    character,
    loadCharacter,
  } = useContext(CharacterContext);
  useEffect(() =>{
    // manually load the only test character for now
    // will revisit this once users are set up
    console.log('loop?');
    loadCharacter('A0771105-CAE2-44FE-9AEB-CBA2D1DFA29B');
  }, []);

  return (
    <div>
      <div>
        Character Sheet
      </div>
      <div>{character.name}</div>
    </div>
  );
};

export default CharacterSheet;
