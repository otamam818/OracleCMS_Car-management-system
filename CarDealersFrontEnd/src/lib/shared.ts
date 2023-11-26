/**
 * @fileoverview some values will need to be shared between components
 *               Among those are values that make sense to be global.
 *               This file aims to store those shared global values
 */

import { checkSigninStatus } from "./api";

/* 
 * keeping a simple word like "passwordHash" may allow attackers to realize
 * that the value actually represents the passwordHash of a user.
 * To prevent that, this global value is used to accurately refer to it while
 * not revealing its purpose to attackers.
*/
export const passwordHashKey = "hjkslabnajsklcdsdalbcsdjacsnaldcnslkjancs"

export type SessionModes = 'SessionExpired' | 'SessionExists' | 'NotSignedIn';

// Find out if a passwordHash already exists
export function navigateOnStart(): Promise<SessionModes> {
  const passwordHash = getPasswordHash();

  if (!passwordHash) {
    return new Promise((resolve) => resolve('NotSignedIn'));
  } else {
    return checkSigninStatus(passwordHash);
  }
}

export function getPasswordHash() {
  return document.cookie
    .split("; ")
    .find((row) => row.startsWith(`${passwordHashKey}=`))
    ?.split("=")[1];
}
