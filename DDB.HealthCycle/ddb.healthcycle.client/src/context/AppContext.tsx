import CharacterState from "./Character/CharacterState";

const AppState = ({ children }: { children: React.ReactNode }) => {
  return (
    <CharacterState>
      {children}
    </CharacterState>
  );
};

export default AppState;
