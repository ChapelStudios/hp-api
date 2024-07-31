import { DamageType } from "../enums/DamageType"

const apiBasePath = `api/PlayerCharacter/`

export const CharacterApiPaths = {
  getCharacter: (id: string) => `${apiBasePath}${id}`,
  healCharacter: (id: string, amount: number) => `${apiBasePath}${id}/heal?amount=${amount}`,
  applyTempHp: (id: string, amount: number) => `${apiBasePath}${id}/apply-temp-hp?amount=${amount}`,
  damageCharacter: (id: string, amount: number, damageType: DamageType) => `${apiBasePath}${id}/damage?amount=${amount}&damageType=${damageType}`,
}
