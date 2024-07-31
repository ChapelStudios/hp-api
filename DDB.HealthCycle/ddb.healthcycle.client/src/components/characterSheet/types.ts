import { DamageType } from "../../enums/DamageType";
import { DefenseType } from "../../enums/DefenseType";
import { Die } from "../../enums/Die";
import { ActorAbilities } from "../../enums/ActorAbilities";

export type PlayerCharacter = {
  name: string;
  id: string;
  level: number;
  hitPoints: PlayerCharacterHealthStats;
  classes: PlayerCharacterClassData[];
  stats: PlayerCharacterAbilities;
  defenses: DefenseList;
};

export type DefenseList = {
  [key in DamageType]: DefenseType;
}

export type PlayerCharacterHealthStats = {
  current: number;
  max: number;
  temp: number;
};

export type PlayerCharacterClassData = {
  name: string;
  classLevel: number;
  HitDiceValue: Die;
};

export type PlayerCharacterAbilities = {
    [key in ActorAbilities]: number;
};
