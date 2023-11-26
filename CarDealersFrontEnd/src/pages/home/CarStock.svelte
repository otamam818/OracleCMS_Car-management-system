<script lang="ts">
    import { tick } from "svelte";
  import { apiPath, executePost } from "../../lib/api";
  import { getPasswordHash } from "../../lib/shared";

  export let stockValue;
  export let CarId;
  let setStockValue = stockValue;
  let tickBg = "#1a1a1a";

  async function handleClick() {
    const PasswordHash = getPasswordHash() as string;

    interface IResponse { message: string }
    const response: IResponse = await executePost(
      new URL(`${apiPath}/Cars/Update/Stock`), 
      {
        CarId,
        NewStockValue: setStockValue,
        PasswordHash
      }
    );

    tickBg = 'green';
    setTimeout(() => {
      tickBg = '#1a1a1a';
    }, 2000)
  }
</script>

  
<td>
  <input
    class="stock-input"
    type="text"
    bind:value={setStockValue}
  />
  <button
    class="tick-button"
    style={
    `background-color: ${tickBg};`
    }
    on:click={handleClick}
  > ✓ </button>
</td>

<style lang="scss">
  .tick-button {
    padding: 6px;
  }

  .stock-input {
    background: #0000002f;
    outline: transparent;
    border: none;
    padding: 10px;
    font-weight: bold;
    width: 50px;
    text-align: center;
    border-radius: 6px;
  }
</style>