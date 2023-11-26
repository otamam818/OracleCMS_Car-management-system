import { writable } from 'svelte/store';

export interface IDealerData {
  carList: Array<{
    carId: number,
    make: string,
    model: string,
    price: number,
    stock: number
  }>,
  dealerName: string
}
export const dealerDataStore = writable<undefined | IDealerData>(undefined);
