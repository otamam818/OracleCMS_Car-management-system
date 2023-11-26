/**
 * @fileoverview The hostname keeps repeating, if this were to be changed in 
 *               every file, then it would be a massive amount of work just to
 *               change deployment channels. Thus this file acts as a hub where
 *               the hostname is very easy to change
 */

import { passwordHashKey, type SessionModes } from "./shared";

const hostname = 'https://localhost:7006'
export const apiPath = `${hostname}/api`

type simpleObject = { [id: string] : string | number };
export async function executePost<T>(
  url: URL,
  searchParams: any,
  requestBody?: simpleObject,
  additionalHeaders?: simpleObject
): Promise<T> {
  url.search = new URLSearchParams(searchParams).toString();

  let body: string = requestBody == undefined
    ? ""
    : JSON.stringify(requestBody);

  let headers = {
    "accept": "*/*",
    ...additionalHeaders
  }

  let response = await fetch(url.href, {
    method: "POST",
    headers,
    body
  });

  return convertToJson(response);
}

async function convertToJson<T>(response: Response) {
  // Try to get the text from the response
  let text = await response.text();
  // Try to parse the text as JSON
  try {
    let json = JSON.parse(text);
    // If successful, return the JSON object
    return json as Promise<T>;
  } catch (error: any) {
    // If not, print the text and the error message
    console.log("The response is not a valid JSON:");
    console.log(text);
    throw new Error(error.message)
  }
}

interface handleSigninResult {
  status: number, // Acts as the HTTP status code
  hashValue: string
}
export async function handleSignin(username: string, password: string) {
  let value = await executePost(
    new URL(`${apiPath}/Dealers/SignIn`), {
    username: username,
    password: password,
  }) as handleSigninResult;
  if (value.status === 200) {
    document.cookie = `${passwordHashKey}=${value.hashValue}; SameSite=Strict; Secure`
    window.location.href = "/home"
  }
}

export async function executeGet(url: URL) {
  let response = await fetch(url.href, {
    method: "GET",
    headers: {
      "accept": "*/*",
    },
  });
  
  return convertToJson(response);
}
interface checkSigninStatusResult {
  status: SessionModes
}
export async function checkSigninStatus(passwordHash: string) {
  const url = new URL(`${apiPath}/Dealers/SignInStatus/${passwordHash}`);
  let value = await executeGet(url) as checkSigninStatusResult;
  return value.status;
}