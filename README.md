# E-Fame-Watch

This is a cool little application, that can take some info from a URL and an XPath, and display it with some statistical data.
Features a normal Column Chart and a Box Chart. 

With the program is an example of some of my workshop items from Steam.

![Error Loading Image](https://m9ogwq.db.files.1drv.com/y4mcqie_1pa29LbHXpfy_nelYiB1vFgaUdMqvzuPpCRzLmeDRrD4KMzb-XTs1WG6WhDHCamC-U6mf-UgSUI5sUZ_cqCG05ilg6ju144BXdppay8KvjRQ_mqzfd7uWkdpXfDxf-Asy8sQAfjouBbp2h10wT1oSIWBN5PXXguaO5j80DBZSCfNaZI6IKm6UWS9x6txyUecNNVvTKB4eu-rfILwg?width=523&height=1004&cropmode=none)
![Error Loading Image](https://ynwqcq.db.files.1drv.com/y4mgpA393FTxT_GPGcEjqUSHORc75bPxyhXRcHchD1-XIhDbvZRs5_EaKp-xfiS0m4ed-ZmniY7BbLLFFrQHMlARcuXW34UlYli2NZ2wESXgOoPp8uwEVKC5_Dyvu8hSvxFb3a9NF1gxb9kCp8MJYgpLU6mJQ9CLGqnUsWSnkXLCtMWrVs3EFn_ur_rpn-U7CPjJjEdq1-1bdb1Xw_28TAzsg?width=263&height=296&cropmode=none)
![Error Loading Image](https://qfc3ew.db.files.1drv.com/y4mhjrsWuq0eHqs4YErSEVw_S-SQ_lAgvbbG0UpEI_Q7H9VCxdXTSY8pBV-SPomQIqA_BI2R6VtUa7d7PSmA9uL_tvrzheuG89DT3XJTGaI9qp2OUy6Vm1fF5JWAxnVbhmUWA0AllUxuIZYNn-bUecrDikhWEzr2YJtZore4GeliJphQ0DgQw2F7irvzzF7PO4SHfjkg-AWHkKNKqMME5Eu9w?width=519&height=296&cropmode=none)
![Error Loading Image](https://mgvftg.db.files.1drv.com/y4m7wo4Twp7g5sbDOQR7sNV8lVdjeyUCPQu0Oy5BzFNVVUuYuZvTDC07r13s7V_wXb55m56TaN7N-PcveRJd1goYSR6H3qO0zBGchi6_IE-on1_G2g9M_cbLWB53WXlftjKzhs8w2qz81c4yhMf_u2Po9yBwR6u11_h4M_5qyXxTULY0t5vHoShM0fYwWlV-UQiJfEc713WKeeC769j-pa_Dg?width=520&height=295&cropmode=none)

This program uses an external package, called HTMLAgilityPack, that can be found here https://html-agility-pack.net/

## How to find an XPath?

In Google Chrome you first find the web page you want to take a value from, in this example i will use one of my own steam workshop items:

![Error Loading Image](https://xnxkiq.db.files.1drv.com/y4mae1Qcd0z7A7BRhW7oeAiSwl9-mHI0nxDvwqCbso9FEZHeng7imyrdiMkiFEfb5zLHbhvmaAVgp2p0ffk56Rs2b0kelh_nONor0lo9lp6jjZTcTYKLRYTOTwmPPqPASkWXnGVCMPRkVwh-3QebilTsEXX9-iOUtAP7cYoiNcWNB62uFk45EWLuQxPmy5ROC6RCzYZAqlZ04SvsvQXGxftGQ?width=1447&height=823&cropmode=none)

After that press F12 to bring up the web dev tools:

![Error Loading Image](https://wwejfg.db.files.1drv.com/y4m5ii-DP4MFuRXNzDYeW1Vh7y_YZJ3C7UIh730TlHhvpYKzAk-qtD0eUwAScRP4npQTk8HTwUPKpxcr3agnlhzNg8HEwSdW2SOe_SWwha5zV0JTWWXZHnbzSdoCELgYV4JIP_y60mys_Bz2PTMx0og1o5yMLBrCpjIGGYEgXeFkc3LNFGy7voP9gQQGbOhYOgCeMxDd54GAWhQnpqA2DE2dw?width=1447&height=822&cropmode=none)

Then click on the inspect button:

![Error Loading Image](https://kjp72w.db.files.1drv.com/y4mhiYkwRESAdMe-_rY25LR5BV4J0uRHnLCWX412DfW8-2UAsqwba_lPqCCgXrh6tq-ZsYCIr7zfINfkTb7YNS3FNDN5qPPzsO9WENAlFx2Qc6InWBrWv7EbNCfHTakg5lMQlnJ_umfrIo1CpECIRKnJSo9oIQ4D4qyDw7YY9_7avvHvgvy5git2ZSLweSMI08p5cbo-Eyl4OA8iBYZTcFFEw?width=1448&height=822&cropmode=none)

Then click on the element in the web page you want:

![Error Loading Image](https://jr5uoq.db.files.1drv.com/y4m8w1v35MPN6UH6jHLboxPFJ3UNmmuyQj-aSVR9l1njz5SY19e5z1BzopZcLtX_9R8VvH8G3pWSk5ATwaBmMGRltgqoNv-Vbm2bsIPefFRZGJw9jDWu_IQRZG1UE7K4dJxl5yfO23W_n25ilirjEWbTANQzZ3Id6y02YGKGcECRczJB87HY8Pn-3_p5fBIqFfH2pr_Mi6g7MeBwAL2y7eCBw?width=1448&height=822&cropmode=none)

Then over in the HTML window right click the element, and under copy, select copy xpath:

![Error Loading Image](https://8qmslg.db.files.1drv.com/y4mBmPGatWQzCsksjUzDAzpHBtJPQmaoi41pnyciVNjnQ_yD54N_zJo2cRzKrepQRtRa7MACLfzIN8eXjibGowGh-aELbUBa1nULAmwCELIcD3DrdksWGll5UsPAX8ZrN9vRw6P269pU4KNrG1ZcyXzEkpTPpoYWtGwiQXiiCWRYvxLXH4qyiV_-gj4rWJy4OFVa1sJHq4uKWNAWJo7hejC4g?width=1448&height=822&cropmode=none)

After this you can just simply paste the xpath to the E-Fame watch program.
