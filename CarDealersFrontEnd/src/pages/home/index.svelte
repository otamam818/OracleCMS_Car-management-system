<script lang="ts">
    import { getPasswordHash, navigateOnStart } from "../../lib/shared";

  // A sample javascript object for the table data
  let cars = [
    { name: "Ford", make: "Mustang", model: "GT", quantity: 3 },
    { name: "Toyota", make: "Corolla", model: "S", quantity: 5 },
    { name: "Honda", make: "Civic", model: "EX", quantity: 4 },
  ];

  async function getAllData() {
    let currPageState = await navigateOnStart();
    if (currPageState !== 'SessionExists') {
      window.location.href = '/login';
    }

    const passwordHash = getPasswordHash() as string;
    console.log({passwordHash});
  }

  getAllData();
</script>

<div class="container">
  <!-- The first component: a title -->
  <h1>SomeTitle</h1>

  <!-- The second component: an input field -->
  <input type="text" placeholder="Enter some text" />

  <!-- The third component: a table -->
  <table>
    <tr>
      <th>Name</th>
      <th>Make</th>
      <th>Model</th>
      <th>Quantity</th>
    </tr>
    {#each cars as car}
    <tr>
      <td>{car.name}</td>
      <td>{car.make}</td>
      <td>{car.model}</td>
      <td>{car.quantity}</td>
    </tr>
    {/each}
  </table>
</div>

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