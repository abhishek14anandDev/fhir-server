﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Core.Features.Search.SearchValues;
using Microsoft.Health.Fhir.Core.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.Core.Features.Search
{
    public class LightweightReferenceToElementResolver : IReferenceToElementResolver
    {
        private readonly IReferenceSearchValueParser _referenceParser;
        private readonly IModelInfoProvider _modelInfoProvider;

        public LightweightReferenceToElementResolver(
            IReferenceSearchValueParser referenceParser,
            IModelInfoProvider modelInfoProvider)
        {
            EnsureArg.IsNotNull(referenceParser, nameof(referenceParser));
            EnsureArg.IsNotNull(modelInfoProvider, nameof(modelInfoProvider));

            _referenceParser = referenceParser;
            _modelInfoProvider = modelInfoProvider;
        }

        public ITypedElement Resolve(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return null;
            }

            ReferenceSearchValue parsed = _referenceParser.Parse(reference);

            if (parsed == null || !_modelInfoProvider.IsKnownResource(parsed.ResourceType))
            {
                return null;
            }

            ISourceNode node = FhirJsonNode.Create(
                JObject.FromObject(
                    new
                    {
                        resourceType = parsed.ResourceType,
                        id = parsed.ResourceId,
                    }));

            return node.ToTypedElement(_modelInfoProvider.StructureDefinitionSummaryProvider);
        }
    }
}
