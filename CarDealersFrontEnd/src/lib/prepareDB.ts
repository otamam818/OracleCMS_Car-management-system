/**
 * @fileoverview A database demonstration can never be complete without
 *               populated data. This file introduces functions that help
 *               create a database with simulated data
 */

import { apiPath, executePost } from "./api";

// Hard-coded values of available cars
interface AvailableCars {
  make: string,
  model: string,
  price: number
}
const availableCars: Array<AvailableCars> = [
  { make: "Toyota",     model: "Corolla",  price: 25000 },
  { make: "Honda",      model: "Civic",    price: 23000 },
  { make: "Ford",       model: "Fiesta",   price: 18000 },
  { make: "Tesla",      model: "Model 3",  price: 45000 },
  { make: "BMW",        model: "3 Series", price: 40000 },
  { make: "Audi",       model: "A4",       price: 38000 },
  { make: "Hyundai",    model: "i30",      price: 21000 },
  { make: "Mazda",      model: "3",        price: 22000 },
  { make: "Nissan",     model: "Leaf",     price: 35000 },
  { make: "Volkswagen", model: "Golf",     price: 24000 }
]

interface AddCompanyResponse {
  status: number,
  id: number
}
export async function prepareCars() {
  const companyIdRecorder: { [id: string]: number } = {};
  for (let i = 0; i < availableCars.length; i++) {
    let { make, model, price } = availableCars[i];

    // Prepare car companies first
    let companyResponse = await executePost(new URL(`${apiPath}/Cars/Company`), {
      Name: make
    }) as AddCompanyResponse;
    
    if (companyResponse.status == 200) {
      companyIdRecorder[make] = companyResponse.id
    }

    // Now prepare the cars
    await executePost(new URL(`${apiPath}/Cars`), {}, {
        id: i+1,
        companyId: companyResponse.id,
        price,
        model
      }, {
        "accept": "text/plain",
        "Content-Type": "application/json"
      }
    );
  }

  // Store this so that it can be used later on in the demonstration
  localStorage.setItem("carIdRecords", JSON.stringify(companyIdRecorder))
}

// Hard-coded values of available dealers
export const carDealers = [
  "CarTime",
  "MotorEmpire",
  "BlissCarWorld",
  "AutoTitan",
  "ShiftingGears",
  "BreezeAuto",
  "WheelsAndDeals",
  "CarPlanet",
  "MightyMotor",
  "EagleAutoSales",
];
interface SignUpDealerResponse {
  hashValue: string,
  status: number
}
// NOTE: This requires prepareCars to be run and completed first before this can be run successfully
export async function prepareDealers() {
  const dealerIdRecorder: { [id: string]: string } = {};
  let idCounter = 1;
  for (let i = 0; i < carDealers.length; i++) {
    let dealerName = carDealers[i];
    // Sign-up the dealers
    console.log(`Executing signUpDealer on ${dealerName}`);
    let companyResponse = await executePost(new URL(`${apiPath}/Dealers/SignUp`), {
      username: dealerName,
      password: dealerName
    }) as SignUpDealerResponse;
    console.log(`Execution successful`);
    
    if (companyResponse.status == 200) {
      dealerIdRecorder[dealerName] = companyResponse.hashValue
    }

    // Now prepare their stock values
    // In this simulation, a company would have anything between 3 to 10 cars
    let numCars = randIntRange(randint(3, 10), 1, 10)
    for (let j = 0; j < numCars.length; j++) {
      let CarId = numCars[j];
      await executePost(new URL(`${apiPath}/Cars/Stock`), {
          // Assumption is that the cars have been added before this
          CarId,
          passwordHash: companyResponse.hashValue,
          Stock: randint(100, 300).toString()
        }, {}, {
          "accept": "text/plain",
        }
      )
    }
  }

  // Store this so that it can be used later on in the demonstration
  localStorage.setItem("dealerIdHashes", JSON.stringify(dealerIdRecorder))
}

// A function that returns a random integer between min (inclusive) and max (inclusive)
function randint(min: number, max: number): number {
  // Check if min and max are numbers or strings of numeric characters
  if (isNaN(min) || isNaN(max)) {
    throw new Error("min and max must be numbers or strings of numeric characters");
  }
  // Convert min and max to numbers and round them down
  min = Math.floor(Number(min));
  max = Math.floor(Number(max));
  // Check if min is less than or equal to max
  if (min > max) {
    throw new Error("min must be less than or equal to max");
  }
  // Return a random integer using Math.floor and Math.random
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

// A function to generate a list of n unique values between min and max inclusive
function randIntRange(n: number, min: number, max: number) {
  // Create an empty array to store the values
  let values: number[] = [];
  // Loop until the array has n elements
  while (values.length < n) {
    // Generate a random integer between min and max
    let value = randint(min, max);
    // Check if the value is already in the array
    if (!values.includes(value)) {
      // If not, push it to the array
      values.push(value);
    }
  }
  // Return the array of values
  return values;
}
