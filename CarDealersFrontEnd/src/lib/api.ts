/**
 * @fileoverview The hostname keeps repeating, if this were to be changed in 
 *               every file, then it would be a massive amount of work just to
 *               change deployment channels. Thus this file acts as a hub where
 *               the hostname is very easy to change
 */

import { passwordHashKey, type SessionModes } from "./shared";

const hostname = 'https://localhost:7006'
const api_path = `${hostname}/api`

interface handleSigninResult {
  status: number, // Acts as the HTTP status code
  hashValue: string
}
export async function handleSignin(username: string, password: string) {
  const url = new URL(`${api_path}/Dealers/SignIn`);

  url.search = new URLSearchParams({
    username: username,
    password: password,
  }).toString();

  let response = await fetch(url.href, {
    method: "POST",
    headers: {
      "accept": "*/*",
    },
    body: ""
  });
  console.log({response})
  let value = await response.json() as handleSigninResult;
  if (value.status === 200) {
    document.cookie = `${passwordHashKey}=${value.hashValue}; SameSite=Strict; Secure`
    window.location.href = "/home"
  }
}

interface checkSigninStatusResult {
  status: SessionModes
}
export async function checkSigninStatus(passwordHash: string) {
  const url = new URL(`${api_path}/Dealers/SignInStatus/${passwordHash}`);

  let response = await fetch(url.href, {
    method: "GET",
    headers: {
      "accept": "*/*",
    },
  });

  let value = await response.json() as checkSigninStatusResult;
  return value.status;
}