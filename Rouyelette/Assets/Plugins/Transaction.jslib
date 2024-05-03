
mergeInto(LibraryManager.library, {

 Credit: async function(accountAddress,amount) 
 {
   
     if (typeof window.ethers !== 'undefined') 
        {
          var address = Pointer_stringify(accountAddress);
          var matAmount = Pointer_stringify(amount);

           console.log("Account address >>" ,address);  
 
           const tx = await window.contract.rewardFunc(address,window.ethers.utils.parseUnits(matAmount));
           console.log("PlaceBid_Credit ==> " , tx);

        } 
        else 
        {
            console.log('Ethers.js is not loaded !!!!!!');
        }
  },

  Deduct: async function(accountAddress,amount) 
 {
   
     if (typeof window.ethers !== 'undefined') 
        {
          console.log("Deduct in process !!!!");

           var matAmount = Pointer_stringify(amount);
           
           try
           {
              const tx = await window.contract.placebid({value : window.ethers.utils.parseUnits(matAmount) });
              console.log("PlaceBid_Deduct ==> " , tx);

              if (typeof nethereumUnityInstance !== 'undefined') 
                {
                    nethereumUnityInstance.SendMessage('WalletConnector', 'Deduct_GetHash',tx.hash);
                } 
                 else 
                {   
                    console.error('nethereumUnityInstance is not defined');
                }
           }
           catch
           {
               
               
               console.log("Deduction rejected !!!");

                if (typeof nethereumUnityInstance !== 'undefined') 
                {
                    nethereumUnityInstance.SendMessage('WalletConnector', 'Deduct_Rejected');
                } 
                else 
                {   
                console.error('nethereumUnityInstance is not defined');
                }
           }
        } 
        else 
        {
            console.log('Ethers.js is not loaded !!!!!!');
        }
  }


  


//     Deduct: function (amount) {

//       const ethers = require('ethers');

//      var amountInString = Pointer_stringify(amount)
//      console.log("amount :",amount ,"amountInString",amountInString);

//     if (typeof window.ethereum !== 'undefined') {
//       window.ethereum.request({ method: 'eth_requestAccounts' })
//         .then(accounts => {
//           if (accounts.length === 0) {
//             console.log('No account is connected.');
//             return;
//           }
//           const account = accounts[0];
//           const params = [{
//             from: account,
//             to: '0x55B62C7f8a159Ef24f2AE4Eb9593e959e92066dc',
//             value: amountInString // Converts the Ether value to wei in hexadecimal
//           }];
//           return window.ethereum.request({
//             method: 'eth_sendTransaction',
//             params: params
//           });
//         })
//         .then(txHash => {
//           console.log('Transaction sent! Hash:', txHash);
//           // You can optionally forward this transaction hash to Unity
//           if (typeof nethereumUnityInstance !== 'undefined') {
//             nethereumUnityInstance.SendMessage('WalletConnector', 'OnTransactionSent', txHash);
//           }
//         })
//         .catch(err => {
//           console.error('Failed to send transaction:', err);
//           // Handle errors by sending an error message to Unity
//           if (typeof nethereumUnityInstance !== 'undefined') {
//             nethereumUnityInstance.SendMessage('WalletConnector', 'OnTransactionError', err.message);
//           }
//         });
//     } else {
//       console.log('Ethereum object not found. Please install MetaMask.');
//     }
//  },

//   Credit: function (amount) {
//      var amountInString = Pointer_stringify(amount)
//      console.log("amount :",amount ,"amountInString",amountInString);

//     if (typeof window.ethereum !== 'undefined') {
//       window.ethereum.request({ method: 'eth_requestAccounts' })
//         .then(accounts => {
//           if (accounts.length === 0) {
//             console.log('No account is connected.');
//             return;
//           }
//           const account = accounts[0];
//           const params = [{
//             from: '0x55B62C7f8a159Ef24f2AE4Eb9593e959e92066dc',
//             to: account,
//             value: amountInString // Converts the Ether value to wei in hexadecimal
//           }];
//           return window.ethereum.request({
//             method: 'eth_sendTransaction',
//             params: params
//           });
//         })
//         .then(txHash => {
//           console.log('Transaction sent! Hash:', txHash);
//           // You can optionally forward this transaction hash to Unity
//           if (typeof nethereumUnityInstance !== 'undefined') {
//             nethereumUnityInstance.SendMessage('WalletConnector', 'OnTransactionSent', txHash);
//           }
//         })
//         .catch(err => {
//           console.error('Failed to send transaction:', err);
//           // Handle errors by sending an error message to Unity
//           if (typeof nethereumUnityInstance !== 'undefined') {
//             nethereumUnityInstance.SendMessage('WalletConnector', 'OnTransactionError', err.message);
//           }
//         });
//     } else {
//       console.log('Ethereum object not found. Please install MetaMask.');
//     }
//  }

});