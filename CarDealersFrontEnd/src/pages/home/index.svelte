<script lang="ts">
  import { apiPath, executeGet, executePost } from "../../lib/api";
  import { getPasswordHash, navigateOnStart } from "../../lib/shared";
    import CarStock from "./CarStock.svelte";
    import Search from "./Search.svelte";
    import { dealerDataStore, type IDealerData } from "./store";


  let dealerData: IDealerData | undefined = undefined;

  dealerDataStore.subscribe(value => {
    dealerData = value;
  })

  async function handleSignOut() {
    // The user's session is still active, we may as well sign them out since
    // they are asking for it
    const passwordHash = getPasswordHash() as string;

    interface IResponse { status: number }
    const response: IResponse = await executePost(
      new URL(`${apiPath}/Dealers/SignOut`), 
      { passwordHash }
    );

    if (response.status == 200) {
      window.location.href = '/login';
    } else {
      console.error("Response status was not 200 in SignOut, something went wrong");
    }
  }

  async function getAllData() {
    let currPageState = await navigateOnStart();
    if (currPageState !== 'SessionExists') {
      window.location.href = '/login';
    }

    const passwordHash = getPasswordHash() as string;
    console.log(passwordHash);
    
    let carList = await executeGet(
      new URL(`${apiPath}/Cars/dealer/${passwordHash}`)) as IDealerData;

    let dealerResponse = await executeGet(
      new URL(`${apiPath}/Dealers/Name/${passwordHash}`)
    ) as { name: string };
    carList.dealerName = dealerResponse.name;
    
    dealerDataStore.set(carList);
  }

  getAllData();
</script>

{#if dealerData === undefined}
<h1> Welcome </h1>
{:else}
<div class="container">
  <h1>{dealerData.dealerName}</h1>
  <Search />
  <table>
    <tr>
      <th>Make</th>
      <th>Model</th>
      <th>Price</th>
      <th>Quantity</th>
    </tr>
    {#each dealerData.carList as car}
    <tr>
      <td>{car.make}</td>
      <td>{car.model}</td>
      <td>$ {car.price}</td>
      <CarStock CarId={car.carId} stockValue={car.stock}/>
    </tr>
    {/each}
  </table>
</div>
{/if}

<button on:click={handleSignOut}>
  SignOut
</button>

<style>
  /* A style to center the components */
  .container {
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  /* A style to make the table look nice */
  table {
    border-collapse: collapse;
    margin: 10px;
  }

  th,
  td {
    border: 1px solid black;
    padding: 5px;
  }
</style>