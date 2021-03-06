﻿// Copyright 2012, Ben Aston (ben@bj.ma).
// 
// This file is part of NFeature.
// 
// NFeature is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// NFeature is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with NFeature.  If not, see <http://www.gnu.org/licenses/>.

namespace NFeature.Test.Slow
{
	using System;
	using Configuration;
	using DefaultImplementations;
	using NUnit.Framework;

	[TestFixture]
	[Category("Slow")]
	public class FeatureEnumExtensionsTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup() {
			var availabilityChecker =
				new FeatureSettingAvailabilityChecker<Feature, EmptyArgs, Tenant>(
					MyAvailabilityCheckFunction);
			var featureSettingRepo = new AppConfigFeatureSettingRepository<Feature, Tenant>();
			var featureSettingService =
				new FeatureSettingService<Feature, Tenant, EmptyArgs>(availabilityChecker,
				                                                      featureSettingRepo);
			var manifestCreationStrategy =
				new ManifestCreationStrategyDefault<Feature, Tenant>(featureSettingRepo,
				                                                     featureSettingService);
			var featureManifestService = new FeatureManifestService<Feature>(manifestCreationStrategy);
			_featureManifest = featureManifestService.GetManifest();
		}

		#endregion

		private IFeatureManifest<Feature> _featureManifest;

		/// <summary>
		/// 	A function to test the availability checking behavior.
		/// </summary>
		private static bool MyAvailabilityCheckFunction(FeatureSetting<Feature, Tenant> s,
		                                                EmptyArgs args) {
			return Enum.GetName(typeof (Feature), s.Feature) == "TestFeatureE";
		}

		[Test]
		public void IsAvailable_WhenTheAvailabilityCheckingFunctionReturnsFalse_ReturnsFalse() {
			Assert.That(!Feature.TestFeatureA.IsAvailable(_featureManifest));
			Assert.That(!Feature.TestFeatureB.IsAvailable(_featureManifest));
			Assert.That(!Feature.TestFeatureC.IsAvailable(_featureManifest));
			Assert.That(!Feature.TestFeatureD.IsAvailable(_featureManifest));
		}

		[Test]
		public void
			IsAvailable_WhenTheAvailabilityCheckingFunctionReturnsTrueAndDependenciesAreOK_ReturnsTrue() {
			Assert.That(Feature.TestFeatureE.IsAvailable(_featureManifest));
		}

		[Test]
		public void Setting_WithFullName_RetrievedOK() {
			Assert.That(
				Feature.TestFeatureE.Setting(FeatureSettingNames.TestFeatureE.AssemblyName,
				                             _featureManifest) ==
				"testFeatureSetting1Value");
		}

		[Test]
		public void Setting_WithoutFullName_RetrievedOK() {
			Assert.That(
				Feature.TestFeatureE.Setting(FeatureSettingNames.TestFeatureE.SimpleSetting,
				                             _featureManifest) ==
				"testFeatureSetting2Value");
		}
	}
}