SmartSnsPublisher
=================

offer a simple and uniform way to publish post to sns, such as SinaWeibo, twitter, facebook etc.  
also offer a elegant(i wish) website that take this advantage.


## to-do
- base function
    + link social account, and store token for a while
    + post message to all linked sites
- open new windows to authenticate social account, auto refresh main page when authentication sucess.
- store user input immediately in localstorage(first choice, because it won't be send with header) or cookies
- auto renew access_token
    + at least can warn people to renew it manually
- support sharing location
- support uploading images
- refactory angular codes