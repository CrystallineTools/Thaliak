use quote::{format_ident, quote};

#[proc_macro_attribute]
pub fn api_version(
    args: proc_macro::TokenStream,
    input: proc_macro::TokenStream,
) -> proc_macro::TokenStream {
    let args = syn::parse_macro_input!(args as syn::AttributeArgs);
    let orig_input = syn::parse_macro_input!(input as syn::Item);

    let mut version = None;
    for arg in args {
        if let syn::NestedMeta::Meta(syn::Meta::NameValue(syn::MetaNameValue {
                path,
                lit: syn::Lit::Str(lit),
                ..
            })) = arg {
            if path.is_ident("version") {
                version = Some(lit.value());
            }
        }
    }

    let input = match orig_input {
        syn::Item::Struct(ref x) => x.clone(),
        _ => panic!("api_version must be applied to a struct"),
    };
    
    let version = version.expect("api_version must have a version argument");
    
    let name = input.ident.clone();
    let rusty_version = format_ident!("ver_{}", version.replace('-', "_"));
    
    // resolve the full path to the type
    let root_node = format_ident!("__ROOT_NODE_{rusty_version}");
    let exec_request_fn = quote! { crate::graphql::#rusty_version::execute_request };

    let output = quote! {
        #orig_input
        
        static #root_node: ::once_cell::sync::Lazy<::std::sync::Arc<::juniper::RootNode<'static,
            #name,
            ::juniper::EmptyMutation<crate::graphql::GqlContext>,
            ::juniper::EmptySubscription<crate::graphql::GqlContext>
        >>> = ::once_cell::sync::Lazy::new(|| ::std::sync::Arc::new(::juniper::RootNode::new(
            #name,
            ::juniper::EmptyMutation::<crate::graphql::GqlContext>::default(),
            ::juniper::EmptySubscription::<crate::graphql::GqlContext>::default(),
        )));
        
        pub fn execute_request(ctx: ::std::sync::Arc<crate::graphql::GqlContext>, req: ::hyper::Request<::hyper::Body>)
            -> ::std::pin::Pin<::std::boxed::Box<
                dyn ::std::future::Future<Output = ::hyper::Response<::hyper::Body>>
                    + ::std::marker::Send
            >> {
            ::std::boxed::Box::pin(::juniper_hyper::graphql((&*(#root_node)).clone(), ctx, req))
        }
        
        crate::graphql::add_api_version!(#version => #exec_request_fn);
    };

    output.into()
}
