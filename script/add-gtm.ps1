Function InsertAfterString
{
    param([string]$text, [string]$find, [string]$insert)

    $start = $text.IndexOf($find) + $find.length
    $beginning = $text.Substring(0, $start)
    $end = $text.Substring($start)
    return -join($beginning, $insert, $end)
}

Function InsertGTM
{
    param ($file)

    $input = Get-Content $file -Raw
    $output = InsertAfterString $input "<head>" "<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start': new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0], j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src= 'https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f); })(window,document,'script','dataLayer','GTM-NQZZFW');</script>"
    $output = InsertAfterString $output "<body onload=`"OnLoad('cs')`">" "<noscript><iframe src=`"https://www.googletagmanager.com/ns.html?id=GTM-NQZZFW`" height=`"0`" width=`"0`" style=`"display:none;visibility:hidden`"></iframe></noscript>"
    Set-Content $file -Value $output -Encoding UTF8
}