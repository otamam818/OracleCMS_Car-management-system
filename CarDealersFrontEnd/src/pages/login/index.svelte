<script lang="ts">
  import { apiPath, executeGet, handleSignin } from "../../lib/api";
    import { carDealers, prepareCars, prepareDealers } from "../../lib/prepareDB";
    import { navigateOnStart } from "../../lib/shared";
  let username = "";
  let password = "";
  let message = "";
  let carDealerList: string[] = [];

  async function assessNavigation() {
    let currPageState = await navigateOnStart();
    if (currPageState === 'SessionExists') {
      window.location.href = '/home';
      return;
    }
    carDealerList = await executeGet(new URL(`${apiPath}/Dealers`)) as string[];
  }

  assessNavigation();
  async function prepare() {
    await prepareCars();
    await prepareDealers();
    carDealerList = carDealers;
  }
</script>

<div class="form">
  <h1>Login</h1>
  <input
    class="input"
    type="text"
    placeholder="Username"
    bind:value={username}
  />
  <input
    class="input"
    type="password"
    placeholder="Password"
    bind:value={password}
  />
  <button class="button" on:click={() => handleSignin(username, password)}>
    Log In
  </button>
  {#if carDealerList.length === 0}
  <button class="button" on:click={prepare}>
    Prepare sample database
  </button>
  {:else}
    <strong> Choose a company </strong>
    <div class="spread company-choices">
    {#each carDealerList as dealerName}
      <button on:click={() => {
        username = dealerName;
        password = dealerName;
      }}> {dealerName} </button>
    {/each}
    </div>
  {/if}
  {#if message}
    <p class="message">{message}</p>
  {/if}
</div>

<style>
  .form {
    width: 300px;
    margin: 0 auto;
  }

  .spread {
    display: flex;
  }

  .company-choices {
    flex-wrap: wrap;
    justify-content: space-between;
    gap: 15px;
  }

  .input {
    display: block;
    width: 100%;
    padding: 10px;
    margin: 10px 0;
    border: 1px solid #ccc;
  }

  .button {
    display: block;
    width: 100%;
    padding: 10px;
    margin: 10px 0;
    background: #007bff;
    color: #fff;
    border: none;
    cursor: pointer;
  }

  .message {
    color: #ff0000;
    text-align: center;
  }
</style>
