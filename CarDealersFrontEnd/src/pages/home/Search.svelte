<script lang="ts">
    import { apiPath, executeGet, executePost } from "../../lib/api";
    import { getPasswordHash } from "../../lib/shared";
    import { dealerDataStore, type IDealerData } from "./store";

  let searchInput = '';
  let searchMode: 'Make' | 'Model' = 'Make';

  let dealerData: IDealerData | undefined = undefined;

  dealerDataStore.subscribe(value => {
    dealerData = value;
  })

  function swapMode() {
    searchMode = searchMode === 'Make'
      ? 'Model'
      : 'Make';
  }

  interface ISearchMake {
    carList: {
      companyId: string,
      carId: number,
      make: string,
      model: string,
      price: number,
      stock: number
    }[]
  }
  async function handleSearch() {
    if (dealerData === undefined) {
      return;
    }

    const passwordHash = getPasswordHash() as string;

    if (searchInput.length === 0) {
      // Return it to its original set of cars
      let reqDealerData = await executeGet(
        new URL(`${apiPath}/Cars/dealer/${passwordHash}`)) as IDealerData;
      dealerDataStore.update(previous => ({
        carList: reqDealerData.carList,
        dealerName: previous?.dealerName as string
      }))

      return;
    }

    let searchList: ISearchMake;
    switch (searchMode) {
      case "Make":
        searchList = await executePost(
          new URL(`${apiPath}/Cars/Search/Make/`),
          {
            passwordHash,
            makeName: searchInput
          }
        ) as ISearchMake;
        break;
      case "Model":
        searchList = await executePost(
          new URL(`${apiPath}/Cars/Search/Model/`),
          {
            passwordHash,
            modelName: searchInput
          }
        ) as ISearchMake;
        break;
    }

    let carList: IDealerData = { carList: [], dealerName: dealerData.dealerName }
    searchList.carList.forEach(value => {
      carList.carList.push({
        make: value.make,
        model: value.model,
        price: value.price,
        stock: value.stock
      })
    });

    dealerDataStore.set(carList);
  }
</script>

<div class="spread">
  <input
    type="text"
    bind:value={searchInput}
    placeholder="Search a value"
    on:input={handleSearch}
  />
  <input
    type="button"
    bind:value={searchMode}
    on:click={swapMode}
  />
</div>

<style lang="scss">
  .spread {
    display: flex;
    background-color: black;
    padding: 8px;
    border-radius: 50px;

    input {
      background-color: transparent;
      outline: none;
      border: none;
    }

    input[type="button"] {
      background-color: midnightblue;
      border-radius: 50px;

      &:hover {
        background-color: royalblue;
      }
    }
  }
</style>